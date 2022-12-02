using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Audio;

[Serializable]
public class FXEnhancer
{
	static private bool GlobalSwitch = true;

	// SOUNDS
	////////////////////////////////////////////////////////////////////////
	public bool playSound = false;

	[Range (1,5)]
	public int audioClipCount=1;
	public AudioClip[] audioClipList = new AudioClip[5];
	public float volumeRangeMin = 0.5f;
	public float volumeRangeMax = 1.0f;
	public float pitchRangeMin = 0.8f;
	public float pitchRangeMax = 1.2f;
	[Range(0.0f, 1.0f)]
	public float playSoundDelay = 0.0f;
	public AudioMixerGroup audiomixerGroup = null;

	// PARTICLES
	////////////////////////////////////////////////////////////////////////
	public bool playParticles = false;
	public ParticleSystem particlesPrefab;

	// PLAY ANIMATION
	////////////////////////////////////////////////////////////////////////
	public bool playAnimation = false;
	public Animation animationToLaunch;

	// PLAY EVEN
	////////////////////////////////////////////////////////////////////////
	public bool playEvent = false;
	public UnityEngine.Events.UnityEvent eventToLaunch;

	// CAMERA SHAKE
	////////////////////////////////////////////////////////////////////////
	public bool cameraShake = false;
	[Range(0.0f, 1.0f)]
	public float shakeAmplitude = 0.3f;
	[Range(0.05f, 1.0f)]
	public float shakeSpring = 0.5f;
	[Range(0.0f, 2.0f)]
	public float shakeRecoil = 0.0f;
	[Range(0.0f, 1.0f)]
	public float shakeDamping = 0.5f;

	// CAMERA ZOOM
	////////////////////////////////////////////////////////////////////////
	public bool cameraZoom = false;
	public float zoomTargetFOV = 45f;
	[Range(0.0f, 3.0f)]
	public float zoomReturnTime = 0.2f;
	public AnimationCurve zoomRamp = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);

	// RECOIL
	////////////////////////////////////////////////////////////////////////
	public bool playRecoil = false;
	public Transform recoilTransform;
	[Range(0.0f, 2.0f)]
	public float recoilAmplitude = 0.5f;
	[Range(0.0f, 3.0f)]
	public float recoilReturnSpeed = 0.2f;

	// CHROMATIC ABBERATIONS
	////////////////////////////////////////////////////////////////////////
	public bool playAbberations = false;
	[Range(0.0f, 1.0f)]
	public float abberationsIntensity = 0.5f;
	[Range(0.0f, 3.0f)]
	public float abberationsTime = 0.2f;
	public AnimationCurve abberationsRamp = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);

	// FREEZE
	////////////////////////////////////////////////////////////////////////
	public bool playFreeze = false;
	[Range(0.0f, 1.0f)]
	public float freezeTime = 0.2f;

	// SFX
	////////////////////////////////////////////////////////////////////////
	public bool playSFX = false;
	[Range(0.0f, 5.0f)]
	public float SFXTime = 2.0f;
	public AnimationCurve pitchRamp = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
	public AnimationCurve cutoffRamp = AnimationCurve.Linear(0.0f, 22000.0f, 1.0f, 22000.0f);
	public AudioMixerGroup SFXMixerGroup = null;

	// TIME SCALE
	////////////////////////////////////////////////////////////////////////
	public bool playTimeScale = false;
	[Range(0.0f, 1.0f)]
	public float relativeTime = 0.2f;
	[Range(0.0f, 3.0f)]
	public float fadeInTime = 0.2f;
	[Range(0.0f, 3.0f)]
	public float stayTime = 0.2f;
	[Range(0.0f, 3.0f)]
	public float fadeOutTime = 0.2f;

	// LIGHT
	////////////////////////////////////////////////////////////////////////
	public bool playLight = false;
	[ColorUsageAttribute(false,true)]
	public Color lightColor = Color.white;
	[Range(0.0f, 10.0f)]
	public float lightRange = 0.2f;
	public AnimationCurve lightRamp = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
	[Range(0.0f, 3.0f)]
	public float lightTime = 1.0f;

	// SCREEN FLASH
	////////////////////////////////////////////////////////////////////////
	public bool playFlash = false;
	[ColorUsageAttribute(false,true)]
	public Color flashColor = Color.white;
	[Range(0.0f, 3.0f)]
	public float flashTime = 0.2f;
	public AnimationCurve flashRamp = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);

	// SPAWN VFX
	////////////////////////////////////////////////////////////////////////
	public bool playVFX = false;
	public GameObject vfxPrefab;
	[Range(0.0f, 3.0f)]
	public float vfxTime = 1.0f;

	// SPAWN PHYSICAL OBJECT
	////////////////////////////////////////////////////////////////////////
	public bool spawnPhysic = false;
	public GameObject physicPrefab;
	public Vector3 physicOffset = Vector3.zero;
	public Vector3 ejectionDir = Vector3.zero;
	[Range(0.0f, 1.0f)]
	public float ejectionRandom = 0.2f;
	[Range(0.0f, 10.0f)]
	public float physicTime = 1.0f;

	// UPDATE GAMEMANAGER
	////////////////////////////////////////////////////////////////////////
	public bool updateGameManager = false;
	public int scoreToAdd;


	// PRIVATE MEMBERS
	////////////////////////////////////////////////////////////////////////

	class AnimatedLight
	{
		public Light light;
		public AnimationCurve animationCurve;
		public float remainingTime;
		public float duration;

		public bool Update()
		{
			remainingTime -= Time.unscaledDeltaTime;
			float relativeTime = remainingTime / duration;

			if (relativeTime<0.0f)
				return true;

			light.intensity = animationCurve.Evaluate(1.0f - relativeTime);
			return false;
		}
	}

	class AnimatedRecoil
	{
		public Transform targetObject;
		public float amplitude;
		public float remainingTime;
		public float duration;
		public Vector3 originalPosition;
		

		public bool Update()
		{
			remainingTime -= Time.unscaledDeltaTime;
			float relativeTime = remainingTime / duration;

			if (relativeTime<0.0f)
			{
				targetObject.localPosition = originalPosition;
				return true;
			}

			targetObject.localPosition = originalPosition - Vector3.forward * relativeTime * amplitude;
			return false;
		}
	}

	static private List<AnimatedLight> animatedLights = new List<AnimatedLight>();
	static private List<AnimatedRecoil> animatedRecoils = new List<AnimatedRecoil>();
	static private List<AnimatedLight> lightsToDestroy = new List<AnimatedLight>();
	static private List<AnimatedRecoil> recoilsToDestroy = new List<AnimatedRecoil>();


	static private Vector2 cameraShakeCurrentSpeed = Vector2.zero;
	static private Vector2 cameraShakeCurrentPosition = Vector2.zero;
	static private float cameraShakeSpring = 0.0f;
	static private Vector3 cameraShakeRecoilSpeed = Vector3.zero;
	static private Vector3 cameraShakeRecoilPosition = Vector3.zero;
	static private float cameraShakeDamping = 0.0f;

	static private float freezeTimeCountdown = 0.0f;
	static private float timeScaleCountdown = 0.0f;
	static private float timeScaleInTime = 0.0f;
	static private float timeScaleOutTime = 0.0f;
	static private float timeScaleStayTime = 0.0f;
	static private float timeScaleRelative = 0.0f;
	static private float currentAbberationsTimeCountdown = 0.0f;
	static private float currentAbberationsDuration = 0.0f;
	static private float currentAbberationsIntensity = 0.0f;
	static private AnimationCurve currentAbberationsRamp;
	static private float defaultAbberationsIntensity = 0.0f;
	static private float currentFlashCountdown = 0.0f;
	static private float currentFlashDuration = 0.0f;
	static private Color currentFlashColor = Color.white;
	static private AnimationCurve currentFlashRamp;
	static private Color defaultFlashColor = Color.white;
	static private float zoomCountdown = 0.0f;
	static private float zoomDuration = 0.0f;
	static private float zoomFOV = 0.0f;
	static private AnimationCurve cameraZoomRamp;
	static private float defaultFOV = 0.0f;
	static private Camera currentCamera;
	static private float currentSFXCountdown = 0.0f;
	static private float currentSFXDuration = 0.0f;
	static private AnimationCurve currentPitchRamp;
	static private AnimationCurve currentCutoffRamp;

	static private UnityEngine.Rendering.PostProcessing.ChromaticAberration PPchroma;
	static private UnityEngine.Rendering.PostProcessing.ColorGrading PPcolor;

	static private AudioMixerGroup currentSFXMixerGroup = null;
	static private AudioMixerGroup MasterMixer=null;
	static private bool NoMasterMixer=false;


	public void Activate(Transform hotPoint)
	{
		if (!GlobalSwitch)
			return;

		// SOUNDS
		////////////////////////////////////////////////////////////////////////
		if (playSound && audioClipList!=null)
		{
			GameObject temporaryGameObject = new GameObject("TemporaryAudio");
			AudioSource audioSource = temporaryGameObject.AddComponent<AudioSource>();
			int clipIndex = UnityEngine.Random.Range(0, audioClipCount);
			temporaryGameObject.transform.position = hotPoint.position;
			audioSource.clip = audioClipList[clipIndex];
			audioSource.pitch = UnityEngine.Random.Range(pitchRangeMin, pitchRangeMax);
			audioSource.volume = UnityEngine.Random.Range(volumeRangeMin, volumeRangeMax);
			if (audiomixerGroup != null)
				audioSource.outputAudioMixerGroup = audiomixerGroup;
			audioSource.PlayDelayed(playSoundDelay); // start the sound
			UnityEngine.Object.Destroy(temporaryGameObject, audioClipList[clipIndex].length * audioSource.pitch);
		}

		// PARTICLES
		////////////////////////////////////////////////////////////////////////
		if(playParticles && particlesPrefab!=null)
		{
			ParticleSystem partSys = UnityEngine.Object.Instantiate(particlesPrefab, hotPoint.position, hotPoint.rotation);
			UnityEngine.Object.Destroy(partSys.gameObject, partSys.main.duration);
		}

		// VFX
		////////////////////////////////////////////////////////////////////////
		if(playVFX && vfxPrefab!=null)
		{
			GameObject gObject = UnityEngine.Object.Instantiate(vfxPrefab, hotPoint.position, hotPoint.rotation);
			UnityEngine.Object.Destroy(gObject, vfxTime);
		}

		// PHYSICAL OBJECT
		////////////////////////////////////////////////////////////////////////
		if(spawnPhysic && physicPrefab!=null)
		{
			GameObject gObject = UnityEngine.Object.Instantiate(physicPrefab, hotPoint.position + 
								hotPoint.localToWorldMatrix.MultiplyVector(physicOffset), hotPoint.rotation);
			Vector3 rndDir = ejectionDir + (ejectionDir.magnitude*0.5f*ejectionRandom*UnityEngine.Random.insideUnitSphere);
			gObject.GetComponent<Rigidbody>().AddForce( hotPoint.localToWorldMatrix.MultiplyVector(rndDir), ForceMode.Acceleration );
			gObject.GetComponent<Rigidbody>().AddTorque (ejectionRandom*UnityEngine.Random.insideUnitSphere*2000f, ForceMode.Acceleration );

			Debug.Log("Spawn pos " + (hotPoint.position + hotPoint.localToWorldMatrix.MultiplyVector(physicOffset)));
			Debug.Log("Spawn dir " + hotPoint.localToWorldMatrix.MultiplyVector(rndDir));
			Debug.Log("Spawn torque " + (ejectionRandom * UnityEngine.Random.insideUnitSphere * 2000f));

			UnityEngine.Object.Destroy(gObject, physicTime);
		}

		// ANIMATION
		////////////////////////////////////////////////////////////////////////
		if(playAnimation && animationToLaunch!=null)
		{
			animationToLaunch.Play();
		}

		// EVENT
		////////////////////////////////////////////////////////////////////////
		if(playEvent && eventToLaunch!=null)
		{
			eventToLaunch.Invoke();
		}

		// CAMERA SHAKE
		////////////////////////////////////////////////////////////////////////
		if (cameraShake)
		{
			cameraShakeRecoilSpeed += hotPoint.forward * shakeRecoil * 20.0f;
			cameraShakeSpring = shakeSpring * 10000.0f;
			cameraShakeDamping = shakeDamping * 20.0f;
			cameraShakeCurrentSpeed += UnityEngine.Random.insideUnitCircle.normalized * shakeAmplitude * 20.0f;
		}

		// CAMERA ZOOM
		////////////////////////////////////////////////////////////////////////
		if (cameraZoom)
		{
			zoomCountdown = zoomDuration = zoomReturnTime;
			zoomFOV = zoomTargetFOV;
			cameraZoomRamp = zoomRamp;
		}

		// FREEZE
		////////////////////////////////////////////////////////////////////////
		if (playFreeze)
		{
			freezeTimeCountdown = Mathf.Max(freezeTime, freezeTimeCountdown);
		}

		// SFX
		////////////////////////////////////////////////////////////////////////
		if (playSFX && currentSFXCountdown < SFXTime)
		{
			currentSFXDuration = currentSFXCountdown = Mathf.Max(SFXTime, currentSFXCountdown);
			currentPitchRamp = pitchRamp;
			currentCutoffRamp = cutoffRamp;
			currentSFXMixerGroup = SFXMixerGroup;
			Debug.Log("Mixer group " + SFXMixerGroup);
		}

		// TIME SCALE
		////////////////////////////////////////////////////////////////////////
		if (playTimeScale)
		{
			timeScaleCountdown = Mathf.Max(fadeInTime+fadeOutTime+stayTime, timeScaleCountdown);
			timeScaleInTime = fadeInTime + 0.001f;
			timeScaleOutTime = fadeOutTime + 0.001f;
			timeScaleStayTime = stayTime + 0.001f;
			timeScaleRelative = relativeTime;
		}

		// CHROMATIC ABBERATIONS
		////////////////////////////////////////////////////////////////////////
		if (playAbberations && currentAbberationsTimeCountdown<abberationsTime)
		{
			currentAbberationsDuration = currentAbberationsTimeCountdown = Mathf.Max(abberationsTime, currentAbberationsTimeCountdown);
			currentAbberationsIntensity = abberationsIntensity;
			currentAbberationsRamp = abberationsRamp;
		}

		// SCREEN FLASH
		////////////////////////////////////////////////////////////////////////
		if (playFlash && currentFlashCountdown<flashTime)
		{
			currentFlashDuration = currentFlashCountdown = Mathf.Max(flashTime, currentFlashCountdown);
			currentFlashColor = flashColor;
			currentFlashRamp = flashRamp;
		}

		// LIGHTS
		////////////////////////////////////////////////////////////////////////
		if (playLight)
		{
			GameObject temporaryGameObject = new GameObject("TemporaryLight");
			Light light = temporaryGameObject.AddComponent<Light>();
			
			light.color = lightColor;
			light.range = lightRange;
			light.transform.position = hotPoint.position;

			AnimatedLight newLight = new AnimatedLight();
			newLight.light = light;
			newLight.animationCurve = lightRamp;
			newLight.remainingTime = newLight.duration = lightTime;

			animatedLights.Add(newLight);
		}

		// RECOILS
		////////////////////////////////////////////////////////////////////////
		if (playRecoil)
		{
			bool found=false;
			for(int i=0;i<animatedRecoils.Count;i++)
			{
				if (animatedRecoils[i].targetObject == recoilTransform)
				{
					animatedRecoils[i].amplitude = recoilAmplitude;
					animatedRecoils[i].duration = animatedRecoils[i].remainingTime = recoilReturnSpeed;
					found = true;
				}
			}

			if (!found)
			{
				AnimatedRecoil newRecoil = new AnimatedRecoil();
				newRecoil.amplitude = recoilAmplitude;
				newRecoil.duration = newRecoil.remainingTime = recoilReturnSpeed;
				newRecoil.targetObject = recoilTransform;
				newRecoil.originalPosition = recoilTransform.localPosition;

				animatedRecoils.Add(newRecoil);
			}
		}

		// GAME MANAGER
		////////////////////////////////////////////////////////////////////////
		if (updateGameManager)
		{
			GameManager.Current.AddToScore(scoreToAdd);
		}
	}

	static public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
			GlobalSwitch =! GlobalSwitch;

		// LIGHTS
		///////////////////////////////////
		for(int i=0;i<animatedLights.Count;i++)
		{
			if (animatedLights[i].Update())
			{
				lightsToDestroy.Add(animatedLights[i]);
			}
		}
		for(int i=0;i<lightsToDestroy.Count;i++)
		{
			GameObject.Destroy(lightsToDestroy[i].light.gameObject);
			animatedLights.Remove(lightsToDestroy[i]);
		}
		lightsToDestroy.Clear();

		// RECOILS
		///////////////////////////////////
		for(int i=0;i<animatedRecoils.Count;i++)
		{
			if (animatedRecoils[i].Update())
			{
				recoilsToDestroy.Add(animatedRecoils[i]);
			}
		}
		for(int i=0;i<recoilsToDestroy.Count;i++)
		{
			animatedRecoils.Remove(recoilsToDestroy[i]);
		}
		recoilsToDestroy.Clear();


		if (freezeTimeCountdown>0.0f)
		{
			freezeTimeCountdown -= Time.unscaledDeltaTime;
			Time.timeScale = 0.0f;
		}
		else if (timeScaleCountdown>0.0f)
		{
			timeScaleCountdown -= Time.unscaledDeltaTime;
			timeScaleCountdown = Mathf.Max(0.0f, timeScaleCountdown);
			if (timeScaleCountdown>(timeScaleOutTime + timeScaleStayTime))
			{
				float relativeTime = 1.0f - (timeScaleCountdown - (timeScaleOutTime + timeScaleStayTime))/timeScaleInTime;
				Time.timeScale = Mathf.Lerp(1.0f, timeScaleRelative, relativeTime);

				////////timeScaleMixerGroup.audioMixer.SetFloat("myParameterName", 0.5f);
				//TimeScaleMixerGroup.audioMixer.
			}
			else if (timeScaleCountdown>timeScaleOutTime)
			{
				Time.timeScale = timeScaleRelative;
			}
			else
			{
				float relativeTime = timeScaleCountdown/timeScaleOutTime;
				Time.timeScale = Mathf.Lerp(1.0f, timeScaleRelative, relativeTime);
			}
		}
		else
			Time.timeScale = 1.0f;

		if (currentAbberationsTimeCountdown>0.0f)
		{
			currentAbberationsTimeCountdown -= Time.unscaledDeltaTime;
			float relativeTime = currentAbberationsTimeCountdown / currentAbberationsDuration;

			relativeTime = currentAbberationsRamp.Evaluate(1.0f - Mathf.Clamp01(relativeTime));

			if (relativeTime<0.0f)
				PPchroma.intensity.value = defaultAbberationsIntensity;
			else
				PPchroma.intensity.value = Mathf.Lerp(defaultAbberationsIntensity,currentAbberationsIntensity, relativeTime);
		}

		if (currentSFXCountdown > 0.0f)
		{
			currentSFXCountdown -= Time.unscaledDeltaTime;
			float relativeTime = currentSFXCountdown / currentSFXDuration;

			float curPitch = currentPitchRamp.Evaluate(1.0f - Mathf.Clamp01(relativeTime));
			float curCutoff = currentCutoffRamp.Evaluate(1.0f - Mathf.Clamp01(relativeTime));

			currentSFXMixerGroup.audioMixer.SetFloat("Pitch", curPitch);
			currentSFXMixerGroup.audioMixer.SetFloat("Cutoff", curCutoff);
		}

		if (currentFlashCountdown>0.0f)
		{
			currentFlashCountdown -= Time.unscaledDeltaTime;
			float relativeTime = currentFlashCountdown / currentFlashDuration;

			relativeTime = currentFlashRamp.Evaluate(1.0f - Mathf.Clamp01(relativeTime));

			if (relativeTime<0.0f)
				PPcolor.colorFilter.value = defaultFlashColor;
			else
				PPcolor.colorFilter.value = Color.Lerp(defaultFlashColor, currentFlashColor, relativeTime);
		}
		if (zoomCountdown>0.0f)
		{
			zoomCountdown -= Time.unscaledDeltaTime;
			float relativeTime = zoomCountdown / zoomDuration;

			relativeTime = cameraZoomRamp.Evaluate(1.0f - Mathf.Clamp01(relativeTime));

			if (relativeTime<0.0f)
				currentCamera.fieldOfView = defaultFOV; 	
			else
				currentCamera.fieldOfView = Mathf.Lerp(currentCamera.fieldOfView, 
								Mathf.Lerp(defaultFOV, zoomFOV, relativeTime), 0.5f);
		}
	}

	static public void Init(GameObject gameObject)
	{
		PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
		volume.profile.TryGetSettings(out PPchroma);
		volume.profile.TryGetSettings(out PPcolor);

		defaultAbberationsIntensity = PPchroma.intensity.value;
		defaultFlashColor = PPcolor.colorFilter.value;
		currentCamera = gameObject.GetComponent<Camera>();
		defaultFOV = currentCamera.fieldOfView;
	}
	static public Vector3 CameraShake(Transform cameraTransform)
	{
		Vector3 delta = Vector3.zero;
	
		cameraShakeCurrentSpeed = cameraShakeCurrentSpeed * Mathf.Max(0.0f, 1.0f - cameraShakeDamping* Time.deltaTime) - cameraShakeCurrentPosition*cameraShakeSpring * Time.deltaTime;
		cameraShakeCurrentPosition +=  cameraShakeCurrentSpeed*Time.deltaTime;
		cameraShakeRecoilSpeed = cameraShakeRecoilSpeed*Mathf.Max(0.0f, 1.0f - cameraShakeDamping* Time.deltaTime) - cameraShakeRecoilPosition*cameraShakeSpring * Time.deltaTime;
		cameraShakeRecoilPosition +=  cameraShakeRecoilSpeed*Time.deltaTime;
		delta += cameraShakeRecoilPosition;
		delta += cameraTransform.right * cameraShakeCurrentPosition.x + cameraTransform.up * cameraShakeCurrentPosition.y;

		return delta;
	}
}