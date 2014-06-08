using UnityEngine;
using System.Collections;

public class CustomSpring : CustomBase {
	
	public float RestLength = 0f;
	public float Strength = 0f;
	public float Damping = 0f;


	public CustomSpring(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2, float restLength, float strength, float damping) {
		this.CustomParticleSystem = particleSystem;
		this.CustomParticleSystem.Springs.Add(this);
		
		this.Particle1 = particle1;
		this.Particle2 = particle2;
		
		this.RestLength = restLength;
		this.Strength = strength;
		this.Damping = damping;
	}

	public override void UpdateGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(Particle1.transform.parent.transform.position + Particle1.Position, Particle2.transform.parent.transform.position + Particle2.Position);
	}

	public override void Delete() {
		Destroy(this, 0.01f);
	}
}
