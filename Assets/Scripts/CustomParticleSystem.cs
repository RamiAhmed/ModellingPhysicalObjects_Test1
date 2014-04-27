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

	private double[] diff(double[] array) {
		double[] returnList = new double[array.Length];
		for (int i = 0; i < array.Length-1; i++) {
			returnList[i] = array[i+1] - array[i]; 
		}
		return returnList;
	}

	private bool getIsAnyValueZero(double[] array) {
		foreach (double point in array) {
			if (point <= 0.0)
				return true;
		}

		return false;
	}

	private double[][] ode4(double[] tspan, double[] y0, params int[] vargin) {
		double[] h = diff(tspan);
		if (getIsAnyValueZero(h)) {
			Debug.LogWarning("Entries of TSPAN are not in order");
		}

		double[] f0 = this.CurrentPhaseSpaceState.ComputeStateDerivate((float)tspan[0], y0, vargin);
		if (f0.Length != y0.Length) {
			Debug.LogWarning("Inconsistent sizes of Y0 and f(t0,y0).");
		}

		int neq = y0.Length;
		int N = tspan.Length;
		double[][] Y = new double[neq][];
		//double[][] F = new double[neq][4];

		for (int g = 0; g < N; g++) {
			Y[g] = new double[N];
		}


		for (int i = 0; i < neq; i++) {
			Y[0][i] = y0[i];
		}

		CustomPhaseSpaceState phaseSpaceState = this.CurrentPhaseSpaceState;

		int x = 0;

		for (int j = 1; j < N; j++) {
			double ti = tspan[j-1];
			double hi = h[j-1];
			double[] yi = new double[neq];
			for (int k = 0; k < neq; k++) {
				yi[k] = Y[k][j-1];
			}

			double[] f1 = phaseSpaceState.ComputeStateDerivate(ti, yi, vargin);

			double[] fyi = new double[yi.Length];
			for (x = 0; x < yi.Length; x++) {
				fyi[x] = yi[x] + 0.5 * hi * f1[x];
			}
			double[] f2 = phaseSpaceState.ComputeStateDerivate(ti+0.5*hi, fyi, vargin);

			for (x = 0; x < yi.Length; x++) {
				fyi[x] = yi[x] + 0.5 * hi * f2[x];
			}
			double[] f3 = phaseSpaceState.ComputeStateDerivate(ti+0.5*hi, fyi, vargin);

			for (x = 0; x < yi.Length; x++) {
				fyi[x] = yi[x] * hi * f3[x];
			}
			double[] f4 = phaseSpaceState.ComputeStateDerivate(tspan[j], fyi, vargin);

			double[] yFinal = new double[yi.Length];
			for (x = 0; x < yi.Length; x++) {
				yFinal[x] = yi[x] + (hi/6)*f1[x] + 2 * f2[x] + 2 * f3[x] + f4[x];
			};

			for (int m = 1; m < yFinal.Length; m++) {
				Y[j][m] = yFinal[m];
			}
		}

		return Y;
	}
	
	private void advanceTime(float deltaTime) {
		CustomPhaseSpaceState phaseSpaceState = this.CurrentPhaseSpaceState;
		if (phaseSpaceState != null) {
			killOldParticles();
			
			double timeStart = this.SystemTime;
			double timeEnd = timeStart + deltaTime;

			//double[] newState = phaseSpaceState.ComputeStateDerivate(timeEnd, phaseSpaceState);
			double[][] phaseSpaceStates = ode4(new double[]{timeStart,timeEnd}, phaseSpaceState.PhaseSpaceState);
			double[] newState = new double[phaseSpaceStates.GetLength(0)];
			for (int i = 0; i < newState.Length; i++) {
				newState[i] = phaseSpaceStates[2][i];
			}
			
			//this.CurrentPhaseSpaceState.PhaseSpaceState = newState;
			phaseSpaceState.SetPhaseSpaceState(newState);
			this.SystemTime = (float)timeEnd;
			
			advanceParticlesAges(deltaTime);
		}
		else {
			Debug.LogWarning("Error in advanceTime: CurrentPhaseSpaceState is null");
		}
		/*killOldParticles();

		AggregateAllForces();

		List<Vector3> velocities = GetParticlesVelocities();
		List<Vector3> accelerations = GetParticlesAccelerations();

		for (int i = 0; i < Particles.Count; i++) {
			CustomParticle particle = Particles[i];

			particle.Position = velocities[i];
			particle.Velocity = accelerations[i];
		}

		advanceParticlesAges(deltaTime);*/
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
