using UnityEngine;
using System.Collections;

public class CustomAttraction : CustomBase {
	
	public CustomParticle Particle1 { get; private set; }
	public CustomParticle Particle2 { get; private set; }
	
	public float Strength { get; private set; }
	public float MinimumDistance { get; private set; }
	
	// Use this for initialization
	void Start () {
		
	}
	
	public void Initialize(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2) {
		Initialize(particleSystem, particle1, particle2, 1f, 0f);
	}
	
	public void Initialize(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2, float strength, float minimumDistance) {
		
		this.CustomParticleSystem = particleSystem;
		
		this.Particle1 = particle1;
		this.Particle2 = particle2;
		
		this.Strength = strength;
		this.MinimumDistance = minimumDistance;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
