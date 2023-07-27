using Microsoft.Xna.Framework;
using MonoGame.Forms.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MonoGame.Forms.DX.Controls
{
    public class SampleControl : MonoGameControl
    {
        private static IPEndPoint ipEnd;
        private static Socket sock;
        private static Socket client;
        public static string screenport = "60000";
        public static int port;
        public static Task task;
        public static SpriteBatch spritebatch;
        private static int width = Screen.PrimaryScreen.Bounds.Width;
        private static int height = Screen.PrimaryScreen.Bounds.Height;
        private static Texture2D texture, texturetemp;
        private static KeyboardState keyState;
        private static int[] wd = { 2, 2, 2, 2, 2 };
        private static int[] wu = { 2, 2, 2, 2, 2 };
        private static void valchanged(int n, bool val)
        {
            if (val)
            {
                if (wd[n] <= 1)
                {
                    wd[n] = wd[n] + 1;
                }
                wu[n] = 0;
            }
            else
            {
                if (wu[n] <= 1)
                {
                    wu[n] = wu[n] + 1;
                }
                wd[n] = 0;
            }
        }
        protected override void Initialize()
        {
            base.Initialize();
            spritebatch = Editor.spriteBatch;
            Connect();
        }
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        protected override void Draw()
        {
            keyState = Keyboard.GetState();
            valchanged(0, keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F5));
            if (wd[0] == 1)
            {
                Form1.Minimize();
            }
            if (task.IsCompleted)
                task = Task.Run(() =>
                {
                    try
                    {
                        byte[] clientData = new byte[1024 * 300];
                        int received = client.Receive(clientData);
                        if (received > 0)
                        {
                            byte[] Data = TrimEnd(clientData);
                            if (Data.Length > 0)
                            {
                                Editor.spriteBatch = spritebatch;
                                texture = byteArrayToTexture(Data);
                                if (texture.Width == width && texture.Height == height)
                                {
                                    texturetemp = texture;
                                    base.Draw();
                                    Editor.spriteBatch.Begin();
                                    Editor.spriteBatch.Draw(texture, new Vector2(0, 0), new Microsoft.Xna.Framework.Rectangle(0, 0, width, height), Microsoft.Xna.Framework.Color.White);
                                    Editor.spriteBatch.End();
                                    texture.Dispose();
                                }
                                else
                                {
                                    base.Draw();
                                    Editor.spriteBatch.Begin();
                                    Editor.spriteBatch.Draw(texturetemp, new Vector2(0, 0), new Microsoft.Xna.Framework.Rectangle(0, 0, width, height), Microsoft.Xna.Framework.Color.White);
                                    Editor.spriteBatch.End();
                                    texture.Dispose();
                                }
                            }
                        }
                    }
                    catch { }
                    System.Threading.Thread.Sleep(40);
                });
        }
        public static void Connect()
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            while (!client.Connected)
            {
                try
                {
                    port = Convert.ToInt32(screenport);
                    ipEnd = new IPEndPoint(IPAddress.Any, port);
                    sock.Blocking = true;
                    sock.Bind(ipEnd);
                    sock.Listen(100);
                    client.Blocking = true;
                    client = sock.Accept();
                }
                catch { }
                System.Threading.Thread.Sleep(1);
            }
            task = Task.Run(() => {});
        }
        public static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);
            Array.Resize(ref array, lastIndex + 1);
            return array;
        }
        private Texture2D byteArrayToTexture(byte[] imageBytes)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                Texture2D tx = Texture2D.FromStream(Editor.graphics, ms);
                ms.Dispose();
                return tx;
            }
        }
    }
}