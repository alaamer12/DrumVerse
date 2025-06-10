#!/usr/bin/env python3
"""
Robust File Size Analyzer with Rich Terminal Display
Shows file sizes with sorting, history, and filtering options.
"""

import os
import sys
import argparse
import fnmatch
from pathlib import Path
from datetime import datetime
from typing import List, Tuple, Optional, Dict, Any
from dataclasses import dataclass
from concurrent.futures import ThreadPoolExecutor, as_completed
import threading

try:
    from rich.console import Console
    from rich.table import Table
    from rich.progress import Progress, TaskID, SpinnerColumn, BarColumn, TextColumn, TimeElapsedColumn
    from rich.panel import Panel
    from rich.text import Text
    from rich.layout import Layout
    from rich.live import Live
    from rich import box
except ImportError:
    print("Error: 'rich' library is required. Install it with: pip install rich")
    sys.exit(1)


@dataclass
class FileInfo:
    """Data class to store file information."""
    path: Path
    size: int
    modified_time: datetime
    is_dir: bool = False
    error: Optional[str] = None


class FileAnalyzer:
    """Main class for analyzing file sizes with rich terminal output."""
    
    def __init__(self):
        self.console = Console()
        self.files: List[FileInfo] = []
        self.total_size = 0
        self.total_files = 0
        self.errors = []
        self.lock = threading.Lock()
    
    def format_size(self, size: int) -> str:
        """Format file size in human-readable format."""
        for unit in ['B', 'KB', 'MB', 'GB', 'TB']:
            if size < 1024.0:
                if unit == 'B':
                    return f"{size:,} {unit}"
                else:
                    return f"{size:.2f} {unit}"
            size /= 1024.0
        return f"{size:.2f} PB"
    
    def should_ignore(self, path: Path, ignore_patterns: List[str]) -> bool:
        """Check if file/directory should be ignored based on patterns."""
        if not ignore_patterns:
            return False
        
        name = path.name
        str_path = str(path)
        
        for pattern in ignore_patterns:
            if fnmatch.fnmatch(name, pattern) or fnmatch.fnmatch(str_path, pattern):
                return True
        return False
    
    def get_file_info(self, path: Path) -> FileInfo:
        """Get information about a single file or directory."""
        try:
            stat = path.stat()
            modified_time = datetime.fromtimestamp(stat.st_mtime)
            
            if path.is_dir():
                # For directories, calculate total size of contents
                total_size = 0
                try:
                    for item in path.rglob('*'):
                        if item.is_file():
                            try:
                                total_size += item.stat().st_size
                            except (OSError, PermissionError):
                                continue
                except (OSError, PermissionError):
                    pass
                
                return FileInfo(
                    path=path,
                    size=total_size,
                    modified_time=modified_time,
                    is_dir=True
                )
            else:
                return FileInfo(
                    path=path,
                    size=stat.st_size,
                    modified_time=modified_time,
                    is_dir=False
                )
        
        except (OSError, PermissionError) as e:
            return FileInfo(
                path=path,
                size=0,
                modified_time=datetime.now(),
                error=str(e)
            )
    
    def scan_directory(self, directory: Path, ignore_patterns: List[str], 
                      max_workers: int = 4) -> None:
        """Scan directory for files with progress tracking."""
        
        # First pass: collect all paths
        all_paths = []
        try:
            with self.console.status("[bold green]Discovering files...") as status:
                for item in directory.iterdir():
                    if not self.should_ignore(item, ignore_patterns):
                        all_paths.append(item)
        except (OSError, PermissionError) as e:
            self.console.print(f"[red]Error accessing directory: {e}[/red]")
            return
        
        if not all_paths:
            self.console.print("[yellow]No files found in directory.[/yellow]")
            return
        
        # Second pass: analyze files with progress bar
        with Progress(
            SpinnerColumn(),
            TextColumn("[progress.description]{task.description}"),
            BarColumn(),
            TextColumn("[progress.percentage]{task.percentage:>3.0f}%"),
            TextColumn("•"),
            TextColumn("[blue]{task.completed}/{task.total}"),
            TextColumn("•"),
            TimeElapsedColumn(),
            console=self.console,
            transient=False
        ) as progress:
            
            task = progress.add_task(
                "[green]Analyzing files...", 
                total=len(all_paths)
            )
            
            # Use ThreadPoolExecutor for concurrent file analysis
            with ThreadPoolExecutor(max_workers=max_workers) as executor:
                # Submit all tasks
                future_to_path = {
                    executor.submit(self.get_file_info, path): path 
                    for path in all_paths
                }
                
                # Process completed tasks
                for future in as_completed(future_to_path):
                    file_info = future.result()
                    
                    with self.lock:
                        if file_info.error:
                            self.errors.append(f"{file_info.path}: {file_info.error}")
                        else:
                            self.files.append(file_info)
                            self.total_size += file_info.size
                            if not file_info.is_dir:
                                self.total_files += 1
                    
                    progress.advance(task)
    
    def sort_files(self, sort_by: str) -> None:
        """Sort files based on the specified criteria."""
        if sort_by == "size_asc":
            self.files.sort(key=lambda x: x.size)
        elif sort_by == "size_desc":
            self.files.sort(key=lambda x: x.size, reverse=True)
        elif sort_by == "name":
            self.files.sort(key=lambda x: x.path.name.lower())
        elif sort_by == "history":
            self.files.sort(key=lambda x: x.modified_time, reverse=True)
    
    def create_results_table(self, show_history: bool) -> Table:
        """Create a rich table with file information."""
        table = Table(
            title=f"File Size Analysis Results",
            box=box.ROUNDED,
            show_header=True,
            header_style="bold magenta"
        )
        
        table.add_column("Type", style="cyan", width=4)
        table.add_column("Name", style="white", min_width=20)
        table.add_column("Size", justify="right", style="green", width=12)
        
        if show_history:
            table.add_column("Modified", style="yellow", width=19)
        
        table.add_column("Path", style="dim", overflow="fold")
        
        for file_info in self.files:
            # Determine file type icon
            if file_info.is_dir:
                type_icon = "📁"
            elif file_info.size == 0:
                type_icon = "📄"
            elif file_info.size > 100 * 1024 * 1024:  # > 100MB
                type_icon = "📊"
            else:
                type_icon = "📄"
            
            # Format size with color coding
            size_str = self.format_size(file_info.size)
            if file_info.size > 1024 * 1024 * 1024:  # > 1GB
                size_str = f"[red]{size_str}[/red]"
            elif file_info.size > 100 * 1024 * 1024:  # > 100MB
                size_str = f"[yellow]{size_str}[/yellow]"
            
            row_data = [
                type_icon,
                file_info.path.name,
                size_str,
            ]
            
            if show_history:
                modified_str = file_info.modified_time.strftime("%Y-%m-%d %H:%M:%S")
                row_data.append(modified_str)
            
            row_data.append(str(file_info.path.parent))
            
            table.add_row(*row_data)
        
        return table
    
    def display_summary(self, directory: Path) -> None:
        """Display summary statistics."""
        summary_table = Table(
            title="Summary Statistics",
            box=box.SIMPLE,
            show_header=False,
            pad_edge=False
        )
        summary_table.add_column("Metric", style="cyan")
        summary_table.add_column("Value", style="green")
        
        summary_table.add_row("Directory", str(directory.absolute()))
        summary_table.add_row("Total Files", f"{self.total_files:,}")
        summary_table.add_row("Total Directories", f"{len([f for f in self.files if f.is_dir]):,}")
        summary_table.add_row("Total Size", self.format_size(self.total_size))
        
        if self.errors:
            summary_table.add_row("Errors", f"{len(self.errors)}")
        
        self.console.print(Panel(summary_table, border_style="blue"))
    
    def display_errors(self) -> None:
        """Display any errors encountered during scanning."""
        if self.errors:
            self.console.print("\n[red]Errors encountered:[/red]")
            for error in self.errors[:10]:  # Show first 10 errors
                self.console.print(f"  [dim]• {error}[/dim]")
            
            if len(self.errors) > 10:
                self.console.print(f"  [dim]... and {len(self.errors) - 10} more errors[/dim]")


def parse_arguments() -> argparse.Namespace:
    """Parse command line arguments."""
    parser = argparse.ArgumentParser(
        description="Analyze file sizes in a directory with rich terminal output",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python filesize.py                           # Analyze current directory
  python filesize.py -p /home/user/Documents   # Analyze specific directory
  python filesize.py --sort size_desc          # Sort by size (largest first)
  python filesize.py --sort name               # Sort by name
  python filesize.py --show-history           # Show modification times
  python filesize.py --ignore "*.tmp" "*.log" # Ignore temp and log files
        """
    )
    
    parser.add_argument(
        "-p", "--path",
        type=str,
        default=".",
        help="Path to directory to analyze (default: current directory)"
    )
    
    parser.add_argument(
        "--sort",
        choices=["size_asc", "size_desc", "name", "history"],
        default="size_desc",
        help="Sort files by: size_asc, size_desc, name, or history (default: size_desc)"
    )
    
    parser.add_argument(
        "--show-history",
        action="store_true",
        help="Show file modification history"
    )
    
    parser.add_argument(
        "--ignore",
        nargs="*",
        default=[],
        help="File/directory names or glob patterns to ignore"
    )
    
    parser.add_argument(
        "--max-workers",
        type=int,
        default=4,
        help="Maximum number of worker threads (default: 4)"
    )
    
    return parser.parse_args()


def main():
    """Main function."""
    args = parse_arguments()
    
    # Validate directory path
    directory = Path(args.path).resolve()
    if not directory.exists():
        print(f"Error: Directory '{directory}' does not exist.")
        sys.exit(1)
    
    if not directory.is_dir():
        print(f"Error: '{directory}' is not a directory.")
        sys.exit(1)
    
    # Create analyzer and run analysis
    analyzer = FileAnalyzer()
    
    try:
        # Display header
        analyzer.console.print(
            Panel(
                f"[bold blue]File Size Analyzer[/bold blue]\n"
                f"Analyzing: [cyan]{directory}[/cyan]",
                border_style="blue"
            )
        )
        
        # Scan directory
        analyzer.scan_directory(directory, args.ignore, args.max_workers)
        
        if not analyzer.files:
            analyzer.console.print("[yellow]No files found to analyze.[/yellow]")
            return
        
        # Sort results
        analyzer.sort_files(args.sort)
        
        # Display results
        analyzer.console.print()
        analyzer.display_summary(directory)
        analyzer.console.print()
        
        # Display file table
        table = analyzer.create_results_table(args.show_history)
        analyzer.console.print(table)
        
        # Display errors if any
        analyzer.display_errors()
        
    except KeyboardInterrupt:
        analyzer.console.print("\n[red]Analysis interrupted by user.[/red]")
        sys.exit(1)
    except Exception as e:
        analyzer.console.print(f"\n[red]Unexpected error: {e}[/red]")
        sys.exit(1)


if __name__ == "__main__":
    main()
