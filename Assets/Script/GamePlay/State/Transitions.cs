using UnityEngine;
using System.Collections;

namespace GamePlay.State {

	public struct Transitions {
		public static string Waiting = "Waiting";
		public static string CountDown = "CountDown";
		public static string Playing = "Playing";
		public static string Paused  = "Paused";
		public static string Finish  = "Finish";
	}
	
}