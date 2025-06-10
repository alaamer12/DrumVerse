using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DrumController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip drumSound;
    [SerializeField] private float volume = 1.0f;
    
    [Header("Input")]
    [SerializeField] private KeyCode keyToPress = KeyCode.None;
    
    [Header("Collision")]
    [SerializeField] private float hitCooldown = 0.1f;
    
    private AudioSource audioSource;
    private float lastHitTime;
    private Collider drumCollider;
    
    private void Start()
    {
        // Get or create audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Setup audio
        audioSource.clip = drumSound;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        
        // Get collider
        drumCollider = GetComponent<Collider>();
        drumCollider.isTrigger = true;
    }
    
    private void Update()
    {
        // Check for key press
        if (keyToPress != KeyCode.None && Input.GetKeyDown(keyToPress))
        {
            PlaySound();
        }
    }
    
    private void OnMouseDown()
    {
        // Play sound when clicked with mouse
        PlaySound();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check cooldown
        if (Time.time - lastHitTime < hitCooldown)
            return;
            
        lastHitTime = Time.time;
        PlaySound();
    }
    
    private void OnCollisionEnter(Collision collision) 
    {
        // Check cooldown
        if (Time.time - lastHitTime < hitCooldown)
            return;
            
        lastHitTime = Time.time;
        PlaySound();
    }
    
    public void PlaySound()
    {
        if (audioSource != null && drumSound != null)
        {
            audioSource.Play();
        }
    }
} 