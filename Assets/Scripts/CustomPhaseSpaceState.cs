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
		for (int i = 1; i <= positions.Count; i++) {
			PhaseSpaceState[3 * i - 3] = positions[i-1].x;
			PhaseSpaceState[3 * i - 2] = positions[i-1].y;
			PhaseSpaceState[3 * i - 1] = positions[i-1].z;
		}

		int particleCount = this.CustomParticleSystem.Particles.Count;
		//double[] velocitiesArray = new double[velocities.Count];
		for (int i = 0; i < velocities.Count; i++) {
			PhaseSpaceState[3 * (i + particleCount) - 3] = velocities[i-1].x;
			PhaseSpaceState[3 * (i + particleCount) - 2] = velocities[i-1].y;
			PhaseSpaceState[3 * (i + particleCount) - 1] = velocities[i-1].z;
		}

		//PhaseSpaceState[0] = positionsArray;
		//PhaseSpaceState[1] = velocitiesArray;
		//PhaseSpaceState[0] = positions.ToArray().ToString().Split();
		//PhaseSpaceState[1] = velocities.ToArray().ToString().Split();

		return PhaseSpaceState;
	}

	public void SetPhaseSpaceState() {
		SetPhaseSpaceState(this.PhaseSpaceState);
	}

	public void SetPhaseSpaceState(double[] phaseSpaceState) {
		int particleCount = this.CustomParticleSystem.Particles.Count;
		if (particleCount > 0) {
			for (int i = 1; i <= particleCount; i++) {
				CustomParticle particle = this.CustomParticleSystem.Particles[i-1];

				particle.Position = new Vector3(
								(float)(phaseSpaceState[3 * i - 3]),
								(float)(phaseSpaceState[3 * i - 2]),
								(float)(phaseSpaceState[3 * i - 1])
								);
				particle.Velocity = new Vector3(
								(float)(phaseSpaceState[3 * (i + particleCount) - 3]),
								(float)(phaseSpaceState[3 * (i + particleCount) - 2]),
								(float)(phaseSpaceState[3 * (i + particleCount) - 1])
								);
			}
		}
	}

	public double[] ComputeStateDerivate(float time, CustomPhaseSpaceState phaseSpaceState) {
		double[] phaseSpace = phaseSpaceState.PhaseSpaceState; // transposed ??
		return ComputeStateDerivate(new double[]{time}, phaseSpace);
	}

	public double[] ComputeStateDerivate(float time, double[] phaseSpaceState, params int[] vargin) {
		return ComputeStateDerivate(new double[]{time}, phaseSpaceState, vargin);
	}

	public double[] ComputeStateDerivate(double time, double[] phaseSpaceState, params int[] vargin) {
		return ComputeStateDerivate(new double[]{time}, phaseSpaceState, vargin);
	}

	public double[] ComputeStateDerivate(double[] time, double[] phaseSpaceState, params int[] vargin) {
		if (this.CustomParticleSystem != null) {
			SetPhaseSpaceState(phaseSpaceState);

			this.CustomParticleSystem.AggregateAllForces();

			List<Vector3> velocities = this.CustomParticleSystem.GetParticlesVelocities();
			List<Vector3> accelerations = this.CustomParticleSystem.GetParticlesAccelerations();

			for (int i = 1; i <= velocities.Count; i++) {
				PhaseSpaceState[3 * i - 3] = velocities[i-1].x;
				PhaseSpaceState[3 * i - 2] = velocities[i-1].y;
				PhaseSpaceState[3 * i - 1] = velocities[i-1].z;
			}
			
			int particleCount = this.CustomParticleSystem.Particles.Count;
			for (int i = 1; i <= accelerations.Count; i++) {
				PhaseSpaceState[3 * (i + particleCount) - 3] = accelerations[i-1].x;
				PhaseSpaceState[3 * (i + particleCount) - 2] = accelerations[i-1].y;
				PhaseSpaceState[3 * (i + particleCount) - 1] = accelerations[i-1].z;
			}


			return PhaseSpaceState; // transposed ??
		}
		else {
			Debug.LogWarning("No particle system set on phase space state");
			return null;
		}
	}
}
