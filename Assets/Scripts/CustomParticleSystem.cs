using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomParticleSystem : MonoBehaviour {
	
	public Vector3 Gravity { get; set; }
	public float Drag { get; set; }
	
	public List<CustomParticle> Particles = new List<CustomParticle>();
	public List<CustomSpring> Springs = new List<CustomSpring>();
	public List<CustomAttraction> Attractions = new List<CustomAttraction>();
	
	public float SystemTime { get; private set; }
	
	public CustomPhaseSpaceState CurrentPhaseSpaceState { get; private set; }
	
	// Use this for initialization
	void Start () {
		this.SystemTime = 0f;

		this.CurrentPhaseSpaceState = new CustomPhaseSpaceState(this);
	}
	
	public void Initialize(Vector3 gravity, float drag) {
		this.Gravity = gravity;
		this.Drag = drag;
	}	
	
	
	// Update is called once per frame
	void Update () {
		advanceTime(Time.deltaTime);
	}
	
	/* PRIVATE METHODS */
	private void killOldParticles() {
		if (Particles.Count > 0) {
			List<CustomParticle> particlesToBeKilled = new List<CustomParticle>();
			
			foreach (CustomParticle particle in Particles) {
				if (particle.LifeSpan > 0 && particle.Age > particle.LifeSpan)
					particlesToBeKilled.Add(particle);
			}
			
			while (particlesToBeKilled.Count > 0) {
				KillParticle(particlesToBeKilled[0]);
				particlesToBeKilled.RemoveAt(0);
			}
		}
	}
	
	private void aggregateAttractionsForces() {
		if (Attractions.Count > 0) {
			foreach (CustomAttraction attract in Attractions) {
				CustomParticle particle1 = attract.Particle1;
				CustomParticle particle2 = attract.Particle2;
				
				Vector3 positionDelta = particle2.Position - particle1.Position;
				float positionDeltaNorm = positionDelta.magnitude;
				
				if (positionDeltaNorm < attract.MinimumDistance) {
					positionDeltaNorm = attract.MinimumDistance;
				}
				
				Vector3 attractionForce = attract.Strength * particle1.Mass * particle2.Mass * positionDelta / positionDeltaNorm / positionDeltaNorm / positionDeltaNorm;
				
				particle1.AddForce(attractionForce);
				particle2.AddForce(-attractionForce);
			}
		}
	}
	
	private void aggregateDragForces() {
		if (Particles.Count > 0) {
			foreach (CustomParticle particle in Particles) {
				Vector3 dragForce = this.Drag * particle.Velocity;
				particle.AddForce(dragForce);
			}
		}
	}
	
	private void aggregateGravityForces() {
		if (Particles.Count > 0) {
			foreach (CustomParticle particle in Particles) {
				Vector3 gravityForce = this.Gravity * particle.Mass;
				particle.AddForce(gravityForce);
			}
		}
	}
	
	private void aggregateSpringsForces() {
		if (Springs.Count > 0) {
			foreach (CustomSpring spring in Springs) {
				CustomParticle particle1 = spring.Particle1;
				CustomParticle particle2 = spring.Particle2;
				
				Vector3 positionDelta = particle2.Position - particle1.Position;
				float positionDeltaNorm = positionDelta.magnitude;
				
				if (positionDeltaNorm < 1f) {
					positionDeltaNorm = 1f;
				}
				
				Vector3 positionDeltaUnit = positionDelta / positionDeltaNorm;
				
				Vector3 springForce = spring.Strength * positionDeltaUnit * (positionDeltaNorm - spring.RestLength);
				
				particle1.AddForce(springForce);
				particle2.AddForce(-springForce);
				
				Vector3 velocityDelta = particle2.Velocity - particle1.Velocity;
				
				Vector3 projectionVelocityDeltaOnPositionDelta = Vector3.Dot(positionDeltaUnit, velocityDelta) * positionDeltaUnit;
				Vector3 dampingForce = spring.Damping * projectionVelocityDeltaOnPositionDelta;
				
				particle1.AddForce(dampingForce);
				particle2.AddForce(-dampingForce);
			}
		}
	}
	
	private void clearParticlesForces() {
		if (Particles.Count > 0) {
			foreach (CustomParticle particle in Particles) {
				particle.ClearForce();
			}
		}
	}
	
	private void advanceParticlesAges(float deltaTime) {
		if (Particles.Count > 0) {
			foreach (CustomParticle particle in Particles) {
				particle.Age += deltaTime;
			}
		}
	}
	
	private void advanceTime(float deltaTime) {
		/*CustomPhaseSpaceState phaseSpaceState = this.CurrentPhaseSpaceState;
		if (phaseSpaceState != null) {
			killOldParticles();
			
			float timeStart = this.SystemTime;
			float timeEnd = timeStart + deltaTime;

			CustomPhaseSpaceState newState = phaseSpaceState.ComputeStateDerivate(phaseSpaceState);
			
			this.CurrentPhaseSpaceState = newState;
			this.SystemTime = timeEnd;
			
			advanceParticlesAges(deltaTime);
		}
		else {
			Debug.LogWarning("Error in advanceTime: CurrentPhaseSpaceState is null");
		}*/
		killOldParticles();

		AggregateAllForces();

		List<Vector3> velocities = GetParticlesVelocities();
		List<Vector3> accelerations = GetParticlesAccelerations();

		for (int i = 0; i < Particles.Count; i++) {
			CustomParticle particle = Particles[i];

			particle.Position = velocities[i];
			particle.Velocity = accelerations[i];
		}

		advanceParticlesAges(deltaTime);
	}
	
	
	
	
	/* PUBLIC METHODS */
	public void AggregateAllForces() {
		clearParticlesForces();
		
		aggregateSpringsForces();
		aggregateAttractionsForces();
		aggregateDragForces();
		aggregateGravityForces();
	}

	
	public List<Vector3> GetParticlesPositions() {
		if (Particles.Count > 0) {
			List<Vector3> particlePositions = new List<Vector3>();
			
			foreach (CustomParticle part in Particles) {
				particlePositions.Add(part.Position);
			}
			
			return particlePositions;
		}
		else
			return null;
	}
	
	public List<Vector3> GetParticlesVelocities() {
		if (Particles.Count > 0) {
			List<Vector3> particleVelocities = new List<Vector3>();
			
			foreach (CustomParticle part in Particles) {
				if (part.Fixed)
					particleVelocities.Add(Vector3.zero);
				else
					particleVelocities.Add(part.Velocity);
			}
			
			return particleVelocities;
		}
		else
			return null;
	}
	
	public List<Vector3> GetParticlesAccelerations() {
		if (Particles.Count > 0) {
			List<Vector3> particleAccelerations = new List<Vector3>();
			
			foreach (CustomParticle part in Particles) {
				Vector3 force = Vector3.zero;
				if (!part.Fixed)
					force = part.Force;
				
				particleAccelerations.Add(force/part.Mass);
			}
			
			return particleAccelerations;
		}
		else
			return null;
	}
	
	
	public void KillAttraction(CustomAttraction attraction) {
		if (Attractions.IndexOf(attraction) >= 0) {
			Attractions.Remove(attraction);
			attraction.Delete();
		}
		else {
			Debug.LogWarning("KillAttraction supplied attraction parameter is invalid: " + attraction.ToString());
		}
	}
	
	public void KillSpring(CustomSpring spring) {
		if (Springs.IndexOf(spring) >= 0) {
			Springs.Remove(spring);
			spring.Delete();
		}
		else {
			Debug.LogWarning("KillSpring supplied spring parameter is invalid: " + spring.ToString());
		}
	}
	
	public void KillParticle(CustomParticle particle) {
		if (Particles.IndexOf(particle) >= 0) {
			
			List<CustomAttraction> attractionsToBeKilled = new List<CustomAttraction>();
			
			foreach (CustomAttraction attract in Attractions) {
				if (particle == attract.Particle1 || particle == attract.Particle2) {
					attractionsToBeKilled.Add(attract);
				}
			}
			
			while (attractionsToBeKilled.Count > 0) {
				KillAttraction(attractionsToBeKilled[0]);
				attractionsToBeKilled.RemoveAt(0);
			}
			
			List<CustomSpring> springsToBeKilled = new List<CustomSpring>();
			
			foreach (CustomSpring spring in Springs) {
				if (particle == spring.Particle1 || particle == spring.Particle2) {
					springsToBeKilled.Add(spring);
				}
			}
			
			while (springsToBeKilled.Count > 0) {
				KillSpring(springsToBeKilled[0]);
				springsToBeKilled.RemoveAt(0);
			}
			
			particle.Delete();
			
		}
		else {
			Debug.LogWarning("KillParticle supplied particle parameter is invalid: " + particle.ToString());
		}
	}
	
}
