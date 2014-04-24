using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomPhaseSpaceState {
	
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
	}
	
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
	
}
