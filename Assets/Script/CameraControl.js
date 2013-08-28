#pragma strict

public var target : Transform;

function Start () {

}

function Update () {
    transform.position.z = target.position.z - 3;
    transform.LookAt(target);
}
