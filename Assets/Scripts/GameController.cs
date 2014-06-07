using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	
	public List<CustomParticleSystem> ParticleSystems = new List<CustomParticleSystem>();

	public bool RenderSpringsInGame = true;
	
	public GameObject ParticleSystemPrefab = null;
	public GameObject ParticlePrefab = null;

	private GameObject playerRef = null;
	private Camera beamCamera = null;


	private CustomParticleSystem addNewParticleSystem(Vector3 systemGravity, float systemDrag) {
		if (ParticleSystemPrefab != null) {
			CustomParticleSystem particleSystem = (Instantiate(ParticleSystemPrefab) as GameObject).GetComponent<CustomParticleSystem>();
			particleSystem.Gravity = systemGravity;
			particleSystem.Drag = systemDrag;

			ParticleSystems.Add(particleSystem);
			return particleSystem;
		}
		else {
			Debug.LogError("ERROR: No particle system prefab set on GameController!");
			return null;
		}
	}

	private CustomParticle addNewParticle(CustomParticleSystem particleSystem, float mass, Vector3 position, Vector3 velocity, bool bFixed, float lifeSpan) {
		if (ParticlePrefab != null) {
			CustomParticle particle = (Instantiate(ParticlePrefab) as GameObject).GetComponent<CustomParticle>();
			particle.Initialize(particleSystem, mass, position, velocity, bFixed, lifeSpan);
			return particle;
		}
		else {
			Debug.LogError("ERROR: No particle prefab set on GameController!");
			return null;
		}
	}


	void Start() {
		playerRef = GameObject.FindGameObjectWithTag("Player");
		if (playerRef == null)
			throw new System.NullReferenceException("Player GameObject could not be found by GameController");
	}

	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.B)) {
			addNewBeamSystem();
		}

		if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) {
			//Debug.Log("DELETE");
			if (this.ParticleSystems.Count > 0) {
				Destroy(ParticleSystems[0].gameObject, 1f);
				this.ParticleSystems.RemoveAt(0);
				Debug.Log("DELETED first particle system");
			}
		}



		if (ParticleSystems.Count > 0) {
			foreach (CustomParticleSystem beamSystem in ParticleSystems) {
				if (beamSystem.Particles.Count > 0) {
					for (int i = 0; i < beamSystem.Particles.Count; i++) {
						CustomParticle particle = beamSystem.Particles[i];

						if (!particle.Fixed) {
							particle.Velocity += Random.insideUnitSphere;
						}
						else {
							particle.transform.position += particle.transform.forward.normalized/4f;
						}
					}
				}
			}

			if (ParticleSystems.Count > 0) {
				CustomParticleSystem lastBeam = ParticleSystems[ParticleSystems.Count-1];
				if (lastBeam != null && lastBeam.Particles.Count > 0) {
					CustomParticle leaderParticle = lastBeam.Particles[0];
					if (leaderParticle != null) {
						Vector3 leaderPos = new Vector3(leaderParticle.Position.x, leaderParticle.Position.y, leaderParticle.Position.z);
						if (lastBeam.GetComponentInChildren<Camera>() == null) {
							beamCamera = (Instantiate(Resources.Load("BeamCamera")) as GameObject).GetComponent<Camera>();
							beamCamera.nearClipPlane = 1f;
							beamCamera.farClipPlane = 500f;
							beamCamera.fieldOfView = 45f;
							beamCamera.clearFlags = CameraClearFlags.SolidColor;
							beamCamera.backgroundColor = Color.black;
							beamCamera.rect = new Rect(0f, 0.65f, 0.65f, 0.35f);
							beamCamera.cullingMask = (1 << LayerMask.NameToLayer("Default"));

							beamCamera.transform.parent = lastBeam.transform;

							beamCamera.transform.localPosition = leaderPos + new Vector3(5f, 20f, 0f);
							beamCamera.transform.LookAt(leaderParticle.transform.position);
							beamCamera.transform.localPosition += new Vector3(0f, 0f, -20f);
						}
						else {
							beamCamera.transform.localPosition = leaderPos + new Vector3(5f, 20f, 0f);
							beamCamera.transform.LookAt(leaderParticle.transform.position);
							beamCamera.transform.localPosition += new Vector3(0f, 0f, -20f);
						}
					}
				}
			}
		}
	}

	private CustomSpring addNewSpring(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2, float restLength, float strength, float damping) {
		CustomSpring newSpring = particleSystem.gameObject.AddComponent<CustomSpring>();
		newSpring.Initialize(particleSystem, particle1, particle2, restLength, strength, damping);
		return newSpring;
	}

	private CustomAttraction addNewAttraction(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2, float strength, float minimumDistance) {
		CustomAttraction newAttraction = particleSystem.gameObject.AddComponent<CustomAttraction>();
		newAttraction.Initialize(particleSystem, particle1, particle2, strength, minimumDistance);
		return newAttraction;
	}


	private void addNewBeamSystem() {
		Vector3 gravity = Vector3.zero;
		float drag = 0.1f;
		int particleCount = 30;
		float particleMass = 10f;
		float particleLifeSpan = 60f;

		CustomParticleSystem beamSystem = addNewParticleSystem(gravity, drag);
		beamSystem.name = "BEAM SYSTEM";
		beamSystem.transform.position = playerRef.transform.position;

		float particleSeparation = 3f;

		CustomParticle leaderParticle = addNewParticle(beamSystem, particleMass, new Vector3(0f, 1f, 0f), Vector3.zero, true, particleLifeSpan);
		leaderParticle.name = "Leader Particle";


		List<CustomParticle> stream1 = new List<CustomParticle>();
		List<CustomParticle> stream2 = new List<CustomParticle>();
		List<CustomParticle> stream3 = new List<CustomParticle>();
		List<CustomParticle> stream4 = new List<CustomParticle>();


		float springRestLength = particleSeparation,
			springStrength = 3f,
			springDamping = 0.75f;
	
		for (int i = 0; i < Mathf.RoundToInt(particleCount/4f); i++) {
			CustomParticle particle = addNewParticle(beamSystem, particleMass, -playerRef.transform.forward * (float)i, Vector3.zero, false, particleLifeSpan);
			particle.name = "Stream_1-" + i.ToString();
			stream1.Add(particle);


			CustomParticle particle2 = addNewParticle(beamSystem, particleMass, -playerRef.transform.forward * (float)i, Vector3.zero, false, particleLifeSpan);
			particle2.name = "Stream_2-" + i.ToString();
			stream2.Add(particle2);


			CustomParticle particle3 = addNewParticle(beamSystem, particleMass, -playerRef.transform.forward * (float)i, Vector3.zero, false, particleLifeSpan);
			particle3.name = "Stream_3-" + i.ToString();
			stream3.Add(particle3);


			CustomParticle particle4 = addNewParticle(beamSystem, particleMass, -playerRef.transform.forward * (float)i, Vector3.zero, false, particleLifeSpan);
			particle4.name = "Stream_4-" + i.ToString();
			stream4.Add(particle4);

			if (i > 0) {
				addNewSpring(beamSystem, stream1[i], stream1[i-1], springRestLength, springStrength, springDamping);
				addNewSpring(beamSystem, stream2[i], stream2[i-1], springRestLength, springStrength, springDamping);
				addNewSpring(beamSystem, stream3[i], stream3[i-1], springRestLength, springStrength, springDamping);
				addNewSpring(beamSystem, stream4[i], stream4[i-1], springRestLength, springStrength, springDamping);
			}
			else {
				addNewSpring(beamSystem, leaderParticle, particle,  springRestLength, springStrength*1.5f, springDamping);
				addNewSpring(beamSystem, leaderParticle, particle2, springRestLength, springStrength*1.5f, springDamping);
				addNewSpring(beamSystem, leaderParticle, particle3, springRestLength, springStrength*1.5f, springDamping);
				addNewSpring(beamSystem, leaderParticle, particle4, springRestLength, springStrength*1.5f, springDamping);
			}
		}
	}
	

}
