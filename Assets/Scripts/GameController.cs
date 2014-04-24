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

	private void AddNewParticleSystem() {
		AddNewParticleSystem(this.Gravity, this.Drag);
	}

	private void AddNewParticleSystem(Vector3 systemGravity, float systemDrag) {
		AddNewParticleSystem(systemGravity, systemDrag, this.ParticleCount, this.ParticleMass, this.ParticleStartPosition, this.ParticleStartVelocity, this.ParticleDefaultFixed, this.ParticleDefaultLifespan);
	}
	
	private void AddNewParticleSystem(Vector3 systemGravity, float systemDrag, int particleCount, float particleMass, Vector3 particleStartPos, Vector3 particleDefaultVelocity, bool particleFixed, float particleLifespan) {
		if (ParticleSystemPrefab != null) {
			CustomParticleSystem particleSystem = (Instantiate(ParticleSystemPrefab) as GameObject).GetComponent<CustomParticleSystem>();
			particleSystem.Gravity = systemGravity;
			particleSystem.Drag = systemDrag;

			for (int i = 0; i < particleCount; i++) {
				addNewParticle(particleSystem, particleMass, particleStartPos, particleDefaultVelocity, particleFixed, particleLifespan);
			}

			ParticleSystems.Add(particleSystem);
		}
		else {
			Debug.LogError("ERROR: No particle system prefab set on GameController!");
		}
	}

	private void addNewParticle(CustomParticleSystem particleSystem, float mass, Vector3 position, Vector3 velocity, bool bFixed, float lifeSpan) {
		if (ParticlePrefab != null) {
			CustomParticle particle = (Instantiate(ParticlePrefab) as GameObject).GetComponent<CustomParticle>();
			particle.Initialize(particleSystem, mass, position, velocity, bFixed, lifeSpan);
		}
		else {
			Debug.LogError("ERROR: No particle prefab set on GameController!");
		}
	}

	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			AddNewParticleSystem();
		}
	}
}
