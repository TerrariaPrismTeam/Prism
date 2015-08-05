using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;

namespace Prism.Debugging
{
    public class DisgustingHacks
    {
#if DEV_BUILD
        /// <summary>
        /// Very ghetto-ly loads an embedded asset through the provided <see cref="ContentManager"/> 
        /// by saving it to a temp file, loading it with the content manager, then deleting the temp file.        
        /// </summary>
        /// <typeparam name="T">Type of content. Same as Content.Load&lt;T&gt;"/></typeparam>
        /// <param name="resourceName">Name of the manifest resource.</param>
        /// <param name="content">The <see cref="ContentManager"/> to load the content through.</param>
        /// <param name="fakeSaveFileName">The file name for the fake file to save. Make sure it doesn't overrite any other file because it gets deleted afterward.</param>
        /// <param name="fakeContentPath">The content path of the fake file saved, relative to the content directory.</param>
        /// <returns></returns>
        public static T LoadResourceThroughContentManager<T>(string resourceName)
        {
            var splitFileName = resourceName.Split('.');
            var fakeContentPath = "";
            var fakeSaveFileName = "";


            for (int i = 0; i < splitFileName.Length; i++)
            {
                if (i == splitFileName.Length - 1)
                    fakeSaveFileName = Path.Combine("Content", fakeContentPath + "." + splitFileName[i]);
                else
                    fakeContentPath += (i == 0 ? "." : "") + splitFileName[i];
            }

            var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            if (File.Exists(fakeSaveFileName))
            {
                File.Delete(fakeSaveFileName);
            }

            var fileStream = File.Create(fakeSaveFileName);

            resourceStream.CopyTo(fileStream);

            resourceStream.Close();
            resourceStream.Dispose();

            fileStream.Close();
            fileStream.Dispose();

            T result = Main.instance.Content.Load<T>(fakeContentPath);

            File.Delete(fakeSaveFileName);

            return result;
            
        }
#endif
    }
}
