using UnityEngine;
using System.Collections;

public class CustomAttraction : CustomBase {
	
	public float Strength = 0f;
	public float MinimumDistance = 0f;
	

	public CustomAttraction(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2, float strength, float minimumDistance) {
		this.CustomParticleSystem = particleSystem;
		this.CustomParticleSystem.Attractions.Add(this);
		
		this.Particle1 = particle1;
		this.Particle2 = particle2;
		
		this.Strength = strength;
		this.MinimumDistance = minimumDistance;
	}

	public override void UpdateGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Particle1.transform.parent.transform.position + Particle1.Position, Particle2.transform.parent.transform.position + Particle2.Position);
	}

	public override void Delete() {
		Destroy(this, 0.01f);
	}
}
