using UnityEngine;
using System.Collections;

public abstract class CustomBase : MonoBehaviour {

	public CustomParticleSystem CustomParticleSystem { get; set; }

	public abstract void Delete();

}
