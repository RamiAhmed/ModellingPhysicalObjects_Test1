using UnityEngine;
using System.Collections;

public class CustomAttraction : CustomBase {
	
	public CustomParticle Particle1 { get; private set; }
	public CustomParticle Particle2 { get; private set; }
	
	public float Strength = 0f;
	public float MinimumDistance = 0f;
	
	public CustomAttraction Initialize(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2) {
		return Initialize(particleSystem, particle1, particle2, 1f, 0.1f);
	}
	
	public CustomAttraction Initialize(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2, float strength, float minimumDistance) {
		this.CustomParticleSystem = particleSystem;
		this.CustomParticleSystem.Attractions.Add(this);
		
		this.Particle1 = particle1;
		this.Particle2 = particle2;
		
		this.Strength = strength;
		this.MinimumDistance = minimumDistance;

		return this;
	}

	public void UpdateGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Particle1.transform.parent.transform.position + Particle1.Position, Particle2.transform.parent.transform.position + Particle2.Position);
	}

	//void OnDrawGizmos() {
	//	UpdateGizmos();
	//}

	public override void Delete() {
		Object.Destroy(this, 0.01f);
	}
}
