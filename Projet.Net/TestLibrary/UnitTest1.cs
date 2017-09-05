/*
 *  Copyright (c) 2014 Raise Partner
 *  26, rue Gustave Eifel, 38000 Grenoble, France
 *  All rights reserved.
 *
 *  This software is the confidential and proprietary information
 *  of Raise Partner. You shall not disclose such Confidential
 *  Information and shall use it only in accordance with the terms
 *  of the licence agreement you entered into with Raise Partner.
 *  
 * */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using TestLibrary;

namespace exempleAppelWRE
{
    [TestClass]
    public class UnitTest1
    {
        
        [TestMethod]
        [DeploymentItem(@"C:\Users\ensimag\Source\Repos\Projet-.Net\Projet.Net\wre-ensimag-c-4.1.dll")]
        public void Main()
        {
            // header
            Debug.WriteLine("******************************");
            Console.WriteLine("*    WREmodelingCov in C#   *");
            Console.WriteLine("******************************");

            // sample data
            double[,] returns = { {0.05, -0.1, 0.6}, {-0.001, -0.4, 0.56}, {0.7, 0.001, 0.12}, {-0.3, 0.2, -0.1},
                                {0.1, 0.2, 0.3}};

            // call WRE via computeCovarianceMatrix encapsulation
            double[,] myCovMatrix =TestUtility.computeCovarianceMatrix(returns);

            // display result
           TestUtility.dispMatrix(myCovMatrix);

            // ending the program            
            Console.WriteLine("\nType enter to exit");
            
        }
    }
}
