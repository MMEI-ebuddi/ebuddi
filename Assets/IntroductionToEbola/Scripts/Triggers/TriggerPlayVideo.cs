//using UnityEngine;
//using System.Collections;
//
//public class TriggerPlayVideo : BaseTrigger {
//			
//	private bool m_bPlayed = false;
//
//    public string MovieFileName = "";
//    public string BundleName = "";
//
//    public Renderer MovieRenderer;
//
//    //   ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();    
//    
//    #if (!UNITY_ANDROID && !UNITY_IOS)
//    private MovieTexture m_MovieTexture = null;
//    #endif
//
//	void OnEnable()
//	{		
//		m_bPlayed = true;
//
//        #if (!UNITY_ANDROID && !UNITY_IOS)            
//            m_MovieTexture = MediaClipManager.LoadMovieTexture(BundleName, MovieFileName);
//            MovieRenderer.material.mainTexture = m_MovieTexture;
//        
//            AudioSource aud = gameObject.AddComponent<AudioSource>();
//            aud.clip = m_MovieTexture.audioClip;
//        
//            m_MovieTexture.Play();
//            aud.Play();
//        #else
//              Handheld.PlayFullScreenMovie(MovieFileName + ".mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);        
//        #endif
//    }
//	
//	public override bool Triggered()
//	{
//        if (m_bPlayed)
//        {
//            #if (!UNITY_ANDROID && !UNITY_IOS)                
//                return !m_MovieTexture.isPlaying;
//            #else
//                // Once fullscreen mobile video has kicked off, control is halted until it's finished, so this will be true when we get back here
//                return true; 
//            #endif
//        }
//        
//        return (false);
//	}
//}
