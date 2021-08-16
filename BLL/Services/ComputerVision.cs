using BLL.Interfaces;
using BLL;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class ComputerVision : IComputerVision
    {
        public string CatVsDogClassifier_(string path)
        {
            //Load sample data
            var sampleData = new CatVsDogClassifier.ModelInput()
            {
                ImageSource = path
            };

            //Load model and predict output
            var result = CatVsDogClassifier.Predict(sampleData);
            return result.Prediction + "on the photo";
        }
    }
}
