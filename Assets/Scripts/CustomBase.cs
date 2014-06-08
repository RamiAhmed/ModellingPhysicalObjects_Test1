using UnityEngine;
using System.Collections;

public abstract class CustomBase : Object {

	public CustomParticleSystem CustomParticleSystem { get; set; }

	public abstract void Delete();

}
