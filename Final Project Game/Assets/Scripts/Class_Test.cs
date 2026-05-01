// using System;
// using System.ComponentModel;
// using System.Linq;
// using Microsoft.Unity.VisualStudio.Editor;
// using UnityEngine;
// using UnityEngine.WSA;

// class AdvanceCShard 
// record Player(string name, int score)
// {
//     public void SubmitScore(){};
// }

// public static class MyExtendtion
// {
//     public static int CountChars(this string s, char target)
//     {
//         int total =0;
//         foreach ( char c in s)
//         {
//             total += 1;
//         }
//         return total;
//     }

//     public static void ChangeOpacity (this Image img, float alpha)
//     {
//         var color = img.color;
//         color.a = alpha;
//         img.color = color;
//     }
// }

// public enum Color
// {
//     RED,
//     GREEN,
//     BLUE,
//     ORANGE,
// }



// struct s
// {
//     private int i;
//     public string GameTag {get; set;}

//     public s (int i, string gt)
//     {
//         this.i = 42;

//     }

// }

// public class Class_Test : MonoBehaviour
// {
//     public GameObject player;
//     private Transform t;
//     private Vector3 playerPos;

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         // t = player.transform;
//         // playerPos = Vector3.zero;
//         // t= new Transform();
//         // playerPos = new Vector3(34,1,1);


//         (string name, int id) = ("Kevin", 85765);
//         (int num1, int num2) = (12,23);
//         (num1,num2) = (num2,num1);

//         // (int,int,float) x = (3,5,3.5f);
//         (int num,int favNum,float amount) x = (3,5,3.5f);
//         x.num =45; 

//         Transform t = this.transform; 
//         Enumerable.Range(0,10);
//         var s = new string('',15);  // 15 spaces 

//         foreach (object c in Enum.GetValues(typeof(Color))){
//             // print(Enum.Parse<Color>(c.ToString()));
//             print((Color)c);
//         }

        
//         Image img;
//         img.color.a = 0.5f

//         string msg = "Hi, you jsut get a high score";
//         print(msg.CountChars('u'));

//         Player p = new("Bod", 65.5 );
//         // Player p2 = new("Bod", 65.5 );
//         Player p2 = p with {name = "RObert"};

//         print ($"P2 name is {p2.name}"); // string interpolation, put the value into the string 
//         print(p.score);
//         print(p2.score);

//         print(p);
//         if (p == p2)
//         {
//             print("payer are euqall");
//         }
//         // p.SubmitScore;
        
//         // switch statement 
//         switch p2.score{
//             case 0: 
//                 print (" you stuck");
//                 break;
//             case 1: 
//                 print (" you got 1 point");
//                 break;
//             default: 
//                 print (" high score");
//                 break;

//         // switch expession
//         var x = p2.score switch
//         {
//             0 => " you stuck";
//             1 => " you got 1 point";
//             _ => " high score"
//         }; 

//         // return a value then put into a function
//         print(p2.score switch
//         {
//             0 => " you stuck";
//             1 => " you got 1 point";
//             _ => " high score"
//         }); 

//         }


//     }



//     // Update is called once per frame
//     void Update()
//     {
//         // t.position;
//         // playerPos;
//     }
// }
