using UnityEngine;
using System.Collections.Generic;
using UnitySpriteCutter;
[RequireComponent( typeof( LineRenderer ) )]
public class LinecastCutter : MonoBehaviour {
	public LayerMask layerMask;

	Vector2 mouseStart;
	GameObject MrNugs;
	int morsels = 3;
	float spriteHeight;
	float spriteLowerBound;

	void Start() {
		MrNugs = GameObject.Find( "MrNugs" );
		spriteHeight = MrNugs.GetComponentInChildren<SpriteRenderer>().bounds.size.y;
		// get bottom bound
		spriteLowerBound = MrNugs.transform.position.y - spriteHeight / 2;

	}
	void Update() {

		if ( Input.GetMouseButtonDown( 0 ) ) {
			mouseStart = Camera.main.ScreenToWorldPoint( Input.mousePosition );
		}

		Vector2 mouseEnd = Camera.main.ScreenToWorldPoint( Input.mousePosition );

		if ( Input.GetMouseButtonUp( 0 ) ) {
			// LinecastCut( mouseStart, mouseEnd, layerMask.value );
		}

		// if I press p, cut the MrNugs in half
		if ( Input.GetKeyDown( KeyCode.P ) ) {
			float offset = morsels / 1.75f;
			Vector2 lineStart = MrNugs.transform.position + new Vector3( -2, (morsels + offset) / spriteHeight, 0 );
			Vector2 lineEnd = MrNugs.transform.position + new Vector3(2, (morsels + offset) / spriteHeight, 0 );
			// randomly modify the angle of the cut
			lineStart.y += Random.Range( -0.5f, 0.5f );
			morsels--;

			// draw the line in the scene view
			GetComponent<LineRenderer>().SetPosition( 0, lineStart );
			GetComponent<LineRenderer>().SetPosition( 1, lineEnd );

			LinecastCut( lineStart, lineEnd, layerMask.value );
		}
	}
	
	public void LinecastCut( Vector2 lineStart, Vector2 lineEnd, int layerMask = Physics2D.AllLayers ) {
		List<GameObject> gameObjectsToCut = new List<GameObject>();
		RaycastHit2D[] hits = Physics2D.LinecastAll( lineStart, lineEnd, layerMask );
		// check if this parent object is in hits, if so, print it
		foreach ( RaycastHit2D hit in hits ) {
			if ( HitCounts( hit )) {
				// check if there is a child
				if ( hit.transform.childCount > 0 ) {
					gameObjectsToCut.Add( hit.transform.GetChild( 0 ).gameObject );
				} else {
					gameObjectsToCut.Add( hit.transform.gameObject );
				}
			}
		}
		
		foreach ( GameObject go in gameObjectsToCut ) {
			SpriteCutterOutput output = SpriteCutter.Cut( new SpriteCutterInput() {
				lineStart = lineStart,
				lineEnd = lineEnd,
				gameObject = go,
				gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_ONE,
			} );

			// SPICY RYAN CODE
			if ( output != null && output.secondSideGameObject != null ) {
				Rigidbody2D newRigidbody = output.secondSideGameObject.AddComponent<Rigidbody2D>();
				// set scale to 0.5
				newRigidbody.gameObject.GetComponent<Animator>().enabled = false;

				if(lineEnd.x - lineStart.x > 0) {
					newRigidbody.AddForce(new Vector2(Random.Range(1, 2), 4) * 1, ForceMode2D.Impulse);
				} else {
					newRigidbody.AddForce(new Vector2(Random.Range(-1, -2), 4) * 1, ForceMode2D.Impulse);
				}

				// add slight rotational force for fun
				newRigidbody.AddTorque(Random.Range(-0.02f, 0.02f) * 1, ForceMode2D.Impulse);


				// disable gravity in a few seconds
				StartCoroutine(DisableGravity(newRigidbody));
			}
		}
	}

	// create a coroutine to disable gravity after a few seconds
	IEnumerator<WaitForSeconds> DisableGravity(Rigidbody2D rigidbody) {
		Destroy(rigidbody.gameObject.GetComponent<Collider2D>());
		yield return new WaitForSeconds(1.25f);
		rigidbody.gravityScale = 0;
		// destroy the rigidbody 
		Destroy(rigidbody);
		// destroy collider
	}

    bool HitCounts( RaycastHit2D hit ) {
		return ( hit.transform.GetComponentInChildren<SpriteRenderer>() != null ||
		         hit.transform.GetComponentInChildren<MeshRenderer>() != null );

	}

}
