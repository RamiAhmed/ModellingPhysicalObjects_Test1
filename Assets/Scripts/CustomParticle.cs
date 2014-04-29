using UnityEngine;
using System.Collections;

public class CustomParticle : CustomBase {
	
	public float Mass = 1f;
	public Vector3 Position {
		get { return this.transform.localPosition; }
		set { this.transform.localPosition = value; }
	}
	
	public Vector3 Velocity = Vector3.zero;
	
	public bool Fixed = false;
	public float LifeSpan = 0f;
	public float Age = 0f;
	
	public Vector3 Force = Vector3.zero;


	public void Initialize(CustomParticleSystem particleSystem) {
		Initialize(particleSystem, 1f, Vector3.zero, Vector3.zero, false, 99999f);
	}
	
	public void Initialize(CustomParticleSystem particleSystem, float mass, Vector3 position, Vector3 velocity, bool bFixed, float lifeSpan) {
		this.CustomParticleSystem = particleSystem;
		this.CustomParticleSystem.Particles.Add(this);

		this.Mass = mass;
		this.Position = position;
		this.Velocity = velocity;
		this.SetFixed(bFixed);
		this.LifeSpan = lifeSpan;

		this.ClearForce();

		this.transform.parent = particleSystem.transform;

		this.name = "Particle " + this.CustomParticleSystem.Particles.IndexOf(this).ToString();
	}
	
	public void ClearForce() {
		this.Force = Vector3.zero;
	}
	
	public void AddForce(Vector3 force) {
		if (!this.Fixed) 
			this.Force += force;
	}
	
	public void SetFixed(bool bFixed) {
		this.Fixed = bFixed;
		
		if (this.Fixed) {
			// Change visuals if fixed ?
			this.transform.localScale = new Vector3(2f, 2f, 2f);
		}
		else {
			
		}
	}

	/*void OnTriggerEnter(Collider other) {
		if (!this.Fixed) {
			if (other.tag.Contains("Particle")) {
				if (!other.GetComponent<CustomParticle>().Fixed) {
					this.Velocity *= -1;
					Debug.Log("Particle collision!");
				}
			}
		}
	}*/
	void OnCollisionEnter(Collision collision) { 
		if (!this.Fixed) {
			if (collision.gameObject.tag.Contains("Particle")) {
				if  (!collision.gameObject.GetComponent<CustomParticle>().Fixed) {
					this.Velocity *= -1;
					Debug.Log("Particle collision!");
				}
			}
		}
	}

}
