using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomPhaseSpaceState {

	public CustomParticleSystem CustomParticleSystem { get; set; }
	/*
	public List<Vector3>[][] PhaseSpaceState = new List<Vector3>[][] {
		new List<Vector3>(),
		new List<Vector3>()
	};
*//*
	public List<List<Vector3>> PhaseSpaceState = new List<List<Vector3>>() {
		new List<Vector3>(),
		new List<Vector3>()
	};*/

	/*public double[][] PhaseSpaceState = new double[2][] {
		new double[1000],
		new double[1000]
	};*/

	public double[] PhaseSpaceState = new double[10000];

	public CustomPhaseSpaceState(CustomParticleSystem particleSystem) {
		this.CustomParticleSystem = particleSystem;
	}

	public double[] GetPhaseSpaceState() {
		List<Vector3> positions = this.CustomParticleSystem.GetParticlesPositions();
		List<Vector3> velocities = this.CustomParticleSystem.GetParticlesVelocities();

		//double[] positionsArray = new double[positions.Count];
		for (int i = 0; i < positions.Count; i++) {
			PhaseSpaceState[3 * i - 2] = positions[i].x;
			PhaseSpaceState[3 * i - 1] = positions[i].y;
			PhaseSpaceState[3 * i - 0] = positions[i].z;
		}

		int particleCount = this.CustomParticleSystem.Particles.Count;
		//double[] velocitiesArray = new double[velocities.Count];
		for (int i = 0; i < velocities.Count; i++) {
			PhaseSpaceState[3 * (i + particleCount) - 2] = velocities[i].x;
			PhaseSpaceState[3 * (i + particleCount) - 1] = velocities[i].y;
			PhaseSpaceState[3 * (i + particleCount) - 0] = velocities[i].z;
		}

		//PhaseSpaceState[0] = positionsArray;
		//PhaseSpaceState[1] = velocitiesArray;
		//PhaseSpaceState[0] = positions.ToArray().ToString().Split();
		//PhaseSpaceState[1] = velocities.ToArray().ToString().Split();

		return PhaseSpaceState;
	}

	public void SetPhaseSpaceState() {
		int particleCount = this.CustomParticleSystem.Particles.Count;
		if (particleCount > 0) {
			for (int i = 0; i < particleCount; i++) {
				CustomParticle particle = this.CustomParticleSystem.Particles[i];

				particle.Position = new Vector3(
								(float)(PhaseSpaceState[3 * i - 2]),
								(float)(PhaseSpaceState[3 * i - 1]),
								(float)(PhaseSpaceState[3 * i])
								);
				particle.Velocity = new Vector3(
								(float)(PhaseSpaceState[3 * (i + particleCount) - 2]),
								(float)(PhaseSpaceState[3 * (i + particleCount) - 1]),
								(float)(PhaseSpaceState[3 * (i + particleCount)])
								);
			}
		}
	}

	public double[] ComputeStateDerivate(float time, CustomPhaseSpaceState phaseSpaceState) {
		if (this.CustomParticleSystem != null) {
			double[] phaseSpace = PhaseSpaceState; // transposed ??

			SetPhaseSpaceState();

			this.CustomParticleSystem.AggregateAllForces();

			List<Vector3> velocities = this.CustomParticleSystem.GetParticlesVelocities();
			List<Vector3> accelerations = this.CustomParticleSystem.GetParticlesAccelerations();
			/*
			PhaseSpaceState[0] = velocities;
			PhaseSpaceState[1] = accelerations;*/

			for (int i = 0; i < velocities.Count; i++) {
				PhaseSpaceState[3 * i - 2] = velocities[i].x;
				PhaseSpaceState[3 * i - 1] = velocities[i].y;
				PhaseSpaceState[3 * i - 0] = velocities[i].z;
			}
			
			int particleCount = this.CustomParticleSystem.Particles.Count;
			//double[] velocitiesArray = new double[velocities.Count];
			for (int i = 0; i < accelerations.Count; i++) {
				PhaseSpaceState[3 * (i + particleCount) - 2] = accelerations[i].x;
				PhaseSpaceState[3 * (i + particleCount) - 1] = accelerations[i].y;
				PhaseSpaceState[3 * (i + particleCount) - 0] = accelerations[i].z;
			}


			return PhaseSpaceState; // transposed ??
		}
		else {
			Debug.LogWarning("No particle system set on phase space state");
			return null;
		}
	}



/*
	public List<Vector3> Positions = new List<Vector3>();
	public List<Vector3> Velocities = new List<Vector3>();

	public CustomParticleSystem CustomParticleSystem { get; set; }

	public CustomPhaseSpaceState(CustomParticleSystem particleSystem) {
		this.CustomParticleSystem = particleSystem;
	}
	
	public void SetPhaseSpaceSet(CustomPhaseSpaceState phaseSpaceState) {
		SetPhaseSpaceSet(phaseSpaceState.Positions, phaseSpaceState.Velocities);
	}
	
	public void SetPhaseSpaceSet(List<Vector3> positions, List<Vector3> velocities) {
		this.Positions = positions;
		this.Velocities = velocities;
		
		if (this.CustomParticleSystem != null) {
			if (this.CustomParticleSystem.Particles.Count > 0) {
				for (int i = 0; i < this.CustomParticleSystem.Particles.Count; i++) {
					CustomParticle particle = this.CustomParticleSystem.Particles[i];

					if (i < positions.Count)
						particle.Position = positions[i];

					if (i < velocities.Count) 
						particle.Velocity = velocities[i];
				}
			}
		}
	}*/
	/*
	public CustomPhaseSpaceState ComputeStateDerivate(CustomPhaseSpaceState phaseSpaceState) {	
		if (this.CustomParticleSystem != null) {
			this.SetPhaseSpaceSet(phaseSpaceState);
			this.CustomParticleSystem.AggregateAllForces();
			
			List<Vector3> velocities = this.CustomParticleSystem.GetParticlesVelocities();
			List<Vector3> accelerations = this.CustomParticleSystem.GetParticlesAccelerations();
			
			CustomPhaseSpaceState newState = new CustomPhaseSpaceState(this.CustomParticleSystem);
			newState.Positions = velocities;
			newState.Velocities = accelerations;
			
			return newState;
		}
		else {
			Debug.LogWarning("No particle system set on phase space state");
			return null;
		}

	}
	*/
}
