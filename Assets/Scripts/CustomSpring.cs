using UnityEngine;
using System.Collections;

public class CustomSpring : CustomBase {
	
	public CustomParticle Particle1 { get; private set; }
	public CustomParticle Particle2 { get; private set; }
	
	public float RestLength = 0f;
	public float Strength = 0f;
	public float Damping = 0f;

	public void Initialize(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2) {
		Initialize(particleSystem, particle1, particle2, 1f, 1f, 0f);
	}
	
	public void Initialize(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2, float restLength, float strength, float damping) {
		this.CustomParticleSystem = particleSystem;
		this.CustomParticleSystem.Springs.Add(this);
		
		this.Particle1 = particle1;
		this.Particle2 = particle2;
		
		this.RestLength = restLength;
		this.Strength = strength;
		this.Damping = damping;

//		this.transform.parent = particleSystem.transform;
	}

	void Update() {

	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(Particle1.transform.parent.transform.position + Particle1.Position, Particle2.transform.parent.transform.position + Particle2.Position);
	}
}
