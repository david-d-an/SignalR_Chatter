using System;
using System.Numerics;

namespace RealtimeClient.SignalR
{
    public class AvatarVectors
    {
        public string Space { get; set; }
        public string PlayerId { get; set; }

        public string PlayerName { get; set; }
        public Vector3 BodyVector { get; set; }
        public Vector2 HeadVector { get; set; }

        public override string ToString()
        {
            return ToString();
        }

        private string ToString(string indent = "  ")
        {
            return string.Join(
                Environment.NewLine,
                $"{indent}Space: {Space} ", 
                $"{indent}Player: ", 
                // $"{indent}{indent}Name: {PlayerName} ", 
                $"{indent}{indent}ID: {PlayerId} ", 
                $"{indent}{indent}Head: ({HeadVector.X},{HeadVector.Y}) ", 
                $"{indent}{indent}Body: ({BodyVector.X},{BodyVector.Y},{BodyVector.Z})"
            );
        }

    }
}
