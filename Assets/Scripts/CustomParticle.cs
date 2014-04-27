using UnityEngine;
using System.Collections;

public class CustomParticle : CustomBase {
	
	public float Mass { get; private set; }
	public Vector3 Position {
		get { return this.transform.position; }
		set { this.transform.position = value; }
	}
	
	public Vector3 Velocity { get; set; }
	
	public bool Fixed { get; private set; }
	public float LifeSpan { get; private set; }
	public float Age { get; set; }
	
	public Vector3 Force { get; private set; }


	public void Initialize(CustomParticleSystem particleSystem) {
		Initialize(particleSystem, 1f, Vector3.zero, Vector3.zero, false, 99999f);
	}
	
	public void Initialize(CustomParticleSystem particleSystem, float mass, Vector3 position, Vector3 velocity, bool bFixed, float lifeSpan) {
		this.CustomParticleSystem = particleSystem;
		this.CustomParticleSystem.Particles.Add(this);

		this.Mass = mass;
		this.Position = position;
		this.Velocity = velocity;
		this.Fixed = bFixed;
		this.LifeSpan = lifeSpan;

		this.transform.parent = particleSystem.transform;
	}
	
	public void ClearForce() {
		this.Force = Vector3.zero;
	}
	
	public void AddForce(Vector3 force) {
		this.Force += force;
	}
	
	public void SetFixed(bool bFixed) {
		this.Fixed = bFixed;
		
		if (this.Fixed) {
			// Change visuals if fixed ?
		}
		else {
			
		}
	}
}
