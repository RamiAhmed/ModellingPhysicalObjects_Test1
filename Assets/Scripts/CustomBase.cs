using UnityEngine;
using System.Collections;

public class CustomBase : MonoBehaviour {

	public CustomParticleSystem CustomParticleSystem { get; set; }

	public void Delete() {
		if (this.gameObject != null) 
			Destroy((Object)this.gameObject, 0.01f);
	}
}
