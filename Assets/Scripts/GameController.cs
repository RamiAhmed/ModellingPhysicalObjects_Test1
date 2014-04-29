using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	
	List<CustomParticleSystem> ParticleSystems = new List<CustomParticleSystem>();
	
	public Vector3 Gravity = Vector3.zero;
	public float Drag = 0f;

	public int ParticleCount = 56;
	public float ParticleMass = 0f;
	public Vector3 ParticleStartPosition = Vector3.zero;
	public Vector3 ParticleStartVelocity = Vector3.zero;
	public bool ParticleDefaultFixed = false;
	public float ParticleDefaultLifespan = 0f;
	
	public GameObject ParticleSystemPrefab = null;
	public GameObject ParticlePrefab = null;
	
	// Use this for initialization
	void Start () {
		
	}

	private CustomParticleSystem AddNewParticleSystem() {
		return AddNewParticleSystem(this.Gravity, this.Drag);
	}

	private CustomParticleSystem AddNewParticleSystem(Vector3 systemGravity, float systemDrag) {
		return AddNewParticleSystem(systemGravity, systemDrag, this.ParticleCount, this.ParticleMass, this.ParticleStartPosition, this.ParticleStartVelocity, this.ParticleDefaultFixed, this.ParticleDefaultLifespan);
	}
	
	private CustomParticleSystem AddNewParticleSystem(Vector3 systemGravity, float systemDrag, int particleCount, float particleMass, Vector3 particleStartPos, Vector3 particleDefaultVelocity, bool particleFixed, float particleLifespan) {
		if (ParticleSystemPrefab != null) {
			CustomParticleSystem particleSystem = (Instantiate(ParticleSystemPrefab) as GameObject).GetComponent<CustomParticleSystem>();
			particleSystem.Gravity = systemGravity;
			particleSystem.Drag = systemDrag;

			if (particleCount > 0) {
				for (int i = 0; i < particleCount; i++) {
					addNewParticle(particleSystem, particleMass, particleStartPos, particleDefaultVelocity, particleFixed, particleLifespan);
				}
			}

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

	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			AddNewParticleSystem();
		}

		if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) {
			addNewParticle(ParticleSystems[ParticleSystems.Count-1], this.ParticleMass, this.ParticleStartPosition, this.ParticleStartVelocity, this.ParticleDefaultFixed, this.ParticleDefaultLifespan);
		}

		if (Input.GetKeyDown(KeyCode.B)) {
			addNewBeam();
		}

		if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) {
			//Debug.Log("DELETE");
			if (this.ParticleSystems.Count > 0) {
				Destroy((Object)ParticleSystems[0].gameObject, 1f);
				this.ParticleSystems.RemoveAt(0);
				Debug.Log("DELETED first particle system");
			}
		}
	}

	private void addNewBeam() {
		Vector3 gravity = new Vector3(0f, 0f, 0f);
		float drag = 0.0f;
		int particleCount = 25;
		float particleMass = 50f;
		Vector3 particleStartPos = new Vector3(0f, 5f, 0f);
		Vector3 particleInitialVelocity = new Vector3(Random.value, Random.value, Random.value) * Random.Range(0.5f, 5f);
		float particleLifeSpan = 1000f + Random.Range(0f, 1000f);
		CustomParticleSystem beamSystem = AddNewParticleSystem(gravity, drag, 0, 0f, Vector3.zero, Vector3.zero, false, 0f);

		CustomParticle leaderParticle = addNewParticle(beamSystem, particleMass, particleStartPos, Vector3.zero, true, 0f);
		leaderParticle.name = "Leader Particle";

		float leaderAttractionStrength = 1f;
		float leaderAttractionMinimumDistance = 5f;
		float springRestLength = 5f;
		float springStrength = 10f;
		float springDamping = 0.8f;

		for (int i = 0; i < particleCount-1; i++) {
			CustomParticle particle = addNewParticle(beamSystem, particleMass, particleStartPos + Random.insideUnitSphere, Vector3.zero, false, particleLifeSpan);

			CustomAttraction leaderAttraction = beamSystem.gameObject.AddComponent<CustomAttraction>();
			leaderAttraction.Initialize(beamSystem, particle, leaderParticle, leaderAttractionStrength, leaderAttractionMinimumDistance);

			//CustomSpring particleSpring = beamSystem.gameObject.AddComponent<CustomSpring>();
			//particleSpring.Initialize(beamSystem, particle, leaderParticle, springRestLength, springStrength, springDamping);
		}



		for (int j = 2; j < particleCount; j++) {
			CustomParticle particle1 = beamSystem.Particles[j-1];
			CustomParticle particle2 = beamSystem.Particles[j];

			CustomSpring particleSpring = beamSystem.gameObject.AddComponent<CustomSpring>();
			particleSpring.Initialize(beamSystem, particle1, particle2, springRestLength, springStrength, springDamping);
		}


	}

}
