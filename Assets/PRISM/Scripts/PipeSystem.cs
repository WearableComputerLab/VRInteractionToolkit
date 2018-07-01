using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSystem : MonoBehaviour {

	public Pipes pipePrefab;

	public int pipeCount;

	private Pipes[] pipes;

	private void Awake () {
		pipes = new Pipes[pipeCount];
		for (int i = 0; i < pipes.Length; i++) {
			Pipes pipe = pipes[i] = Instantiate<Pipes>(pipePrefab);
			pipe.transform.SetParent(transform, false);
			if (i > 0) {
				pipe.AlignWith(pipes[i - 1]);
			}
		}
	}
}