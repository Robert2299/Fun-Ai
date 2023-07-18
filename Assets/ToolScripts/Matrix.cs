﻿using UnityEngine;


namespace nfs.tools {

    ///<summary>
	/// Standard math matrix class.
    /// Can be multiplied or added with another matrix.
    /// Can be set to different predefined matrix.
    /// Can be randomized for synapse initialization.
	///</summary>
    public class Matrix {

        // the matrix itself as an array of array
        private float[][] matrix;

        /// <summary>
        /// The number of rows I.
        /// </summary>
        /// <value>The i.</value>
		public int I { private set; get; }
        /// <summary>
        /// The number of column J.
        /// </summary>
        /// <value>The j.</value>
		public int J { private set; get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="nfs.tools.Matrix"/> class.
        /// </summary>
        /// <param name="i">The number of rows.</param>
        /// <param name="j">The number of column.</param>
        public Matrix(int i, int j) {
            I = i;
            J = j;

            matrix = new float[i][];
			
			for(int k = 0; k < i; k++) {
                matrix[k] = new float[j];
            }
        }

        ///<summary>
	    /// Standard matrix multiply.
        /// Return a matrix of size I from matrix A and J from matrix B
        /// after doing a weighted sum of A lines times B colums.
        /// If there is a mismatch of size, the matrix A will be returned.
        /// Can be set to normalize to get values of each cell devided by the length of the line.
        /// If matrix B has value 01 then the new one will as well.
	    ///</summary>
		public static Matrix Multiply (Matrix matrixA, Matrix matrixB, bool normalize = false) {
            Debug.Assert(matrixA.J == matrixB.I, "Matrix multiplication error due to size missmatch.");

            Matrix resultMatrix = new Matrix(matrixA.I, matrixB.J);

            for (int i = 0; i < matrixA.I; i++) {
                for(int j = 0; j < matrixB.J; j++) {

                    float weightedSum = 0f;
                    for(int k = 0; k < matrixA.J; k ++) {
                        weightedSum += matrixA.matrix[i][k] * matrixB.matrix[k][j];
                    }
                    resultMatrix.matrix[i][j] = normalize? weightedSum/matrixA.J : weightedSum;
                }
            }
            return resultMatrix;
        }

        ///<summary>
	    /// Standard matrix addition. Returns a new matrix.
        /// IF there is a mismatch of size, the matrix A will be returned.
	    ///</summary>
        public static Matrix Add (Matrix matrixA, Matrix matrixB) {
            Debug.Assert(CheckDimention(matrixA, matrixB), "Matrix addition error due to size missmatch.");

            Matrix newMat = new Matrix(matrixA.I, matrixA.J);

            for (int i = 0; i < matrixA.I; i++) {
                for(int j = 0; j < matrixA.J; j++) {
                    float sum = matrixA.matrix[i][j] + matrixB.matrix[i][j];
                    newMat.matrix[i][j] = sum;
                }
            }

            return newMat;
        }

        ///<summary>
	    /// Check if another matrix is of the same dimention as this one.
	    ///</summary>
        public static bool CheckDimention (Matrix matrixA, Matrix matrixB) {
            return (matrixA.I == matrixB.I && matrixA.J == matrixB.J) ? true : false;
        }

        ///<summary>
	    /// Check if another matrix is of the same dimention as this one.
	    ///</summary>
        public static bool CheckDimention (Matrix matrixA, float[][] matrixB) {
            return (matrixA.I == matrixB.Length && matrixA.J == matrixB[0].Length) ? true : false;
        }

        ///<summary>
	    /// Check if another matrix is of the same dimention as this one.
	    ///</summary>
        public static bool CheckDimention (float[][] matrixA, float[][] matrixB) {
            return (matrixA.Length == matrixB.Length && matrixA[0].Length == matrixB[0].Length) ? true : false;
        }

		/// <summary>
		/// Standards the synapse range.
		/// </summary>
		/// <returns>The synapse range.</returns>
		/// <param name="matJ">Mat j.</param>
        public static float StandardSynapseRange (int matJ) {
            return 2f / Mathf.Sqrt(matJ);
        }

        ///<summary>
	    /// Return a deep clone of the original Matrix.
	    ///</summary>
        public Matrix GetClone() {
            Matrix clone = new Matrix(this.I, this.J);
            clone.SetAllValues(this);
            return clone;
        }

        ///<summary>
	    /// Set all values to 0.
	    ///</summary>
        public Matrix SetToZero() {
            for (int i = 0; i < I; i++) {
                for(int j = 0; j < J; j++) {
                    matrix[i][j] = 0f;
                }
            }
            return this;
        }

        ///<summary>
	    /// Set all values to 1.
	    ///</summary>
        public Matrix SetToOne() {
            for (int i = 0; i < I; i++) {
                for(int j = 0; j < J; j++) {
                    matrix[i][j] = 1f;
                }
            }
            return this;
        }

        ///<summary>
	    /// Set diagonal values to 1 and others to 0.
	    ///</summary>
        public Matrix SetToIdentiy() {
            for (int i = 0; i < I; i++) {
                for(int j = 0; j < J; j++) {
                    if (i==j)
                        matrix[i][j] = 1f;
                    else
                        matrix[i][j] = 0f;
                }
            }
            return this;
        }

        ///<summary>
	    /// Randomize all values with within a range dependant on matrix dimension for synapses.
	    ///</summary>
        // weights range from here http://stats.stackexchange.com/questions/47590/what-are-good-initial-weights-in-a-neural-network
        public Matrix SetAsSynapse() {
            float weightRange = StandardSynapseRange(this.J);
            for (int i = 0; i < I; i++) {
                for(int j = 0; j < J; j++) {
                    matrix[i][j] = Random.Range(-weightRange, weightRange);
                }
            }
            return this;
        }

        ///<summary>
	    /// Returns all float values, same as Mtx.
	    ///</summary>
        public float[][] GetAllValues () {
            return matrix;
        }

        ///<summary>
	    /// Returns all float values from a requested line.
	    ///</summary>
        public float[] GetLineValues(int line = 0) {
            Debug.Assert(line <= I, "There is no line " + line + " in this matrix.");

            float[] lineValues = new float[J];

            for(int j=0; j<J; j++){
                lineValues[j] = matrix[line][j];
            }
            return lineValues;
        }

        ///<summary>
	    /// Returns all float values from a requested column.
	    ///</summary>
        public float[] GetColumnValues(int col = 0) {
            Debug.Assert(col <= J, "There is no col " + col + " in this matrix.");
            
            float[] colValues = new float[I];

            for(int i=0; i<I; i++){
                colValues[i] = matrix[i][col];
            }
            return colValues;
        }

        ///<summary>
	    /// Get one specific value in the matrix.
	    ///</summary>
        public float GetValue(int line, int col) {
            Debug.Assert(line <= I && col <= J, "There is no col " + col + " or line " + line + " for this matrix.");

            return matrix[line][col];
        }

        ///<summary>
	    /// Set all values from another matrix.
	    ///</summary>
        public void SetAllValues(Matrix other, bool ignoreMissmatch = false) {
            Debug.Assert(ignoreMissmatch || CheckDimention(this, other), "Matrix dimention not equal, cannot copy values. Do you want to ignore missmatch?");

            for (int i = 0; i < Mathf.Min(I, other.I); i++) {
                for (int j = 0; j < Mathf.Min(J, other.J); j++) {
                    matrix[i][j] = other.matrix[i][j];
                }
            }
        }

        ///<summary>
	    /// Set all values from another matrix.
	    ///</summary>
        public void SetAllValues(float[][] other, bool ignoreMissmatch = false) {
            Debug.Assert(ignoreMissmatch || CheckDimention(this, other), "Matrix dimention not equal, cannot copy values. Do you want to ignore missmatch?");

            for (int i = 0; i < Mathf.Min(I, other.Length); i++) {
                for (int j = 0; j < Mathf.Min(J, other[0].Length); j++) {
                    matrix[i][j] = matrix[i][j];
                }
            }

        }


        ///<summary>
	    /// Set all float values of a line.
        /// It is possible to ignore size missmatch.
	    ///</summary>
        public void SetLineValues (int line, float[] values, bool ignoreMissmatch = false) {
            Debug.Assert(line <= I && values.Length <= this.J, "There is no line " + line + " or the number of columns mismatch (" + values.Length + " vs " + this.J + ") for this matrix.");
            Debug.Assert(J == values.Length || ignoreMissmatch, "Array given not equal to size of line: " + values.Length + " vs " + this.J + ". Do you want to ignore missmatch?");

            for(int j=0; j<Mathf.Min(J, values.Length); j++){
                matrix[line][j] = values[j];
            }
        }

        ///<summary>
	    /// Set all float values of a column.
        /// It is possible to ignore size missmatch.
	    ///</summary>
        public void SetColumnValues (int col, float[] values, bool ignoreMissmatch = false) {
            Debug.Assert(col <= J && values.Length <= this.I, "There is no col " + col + " or the number of lines mismatch (" + values.Length + " vs " + this.I + ") for this matrix.");
            Debug.Assert(I == values.Length || ignoreMissmatch, "Array given not equal to size of column: " + values.Length + " vs " + this.I + ". Do you want to ignore missmatch?");

            for(int i=0; i<Mathf.Min(I, values.Length); i++){
                matrix[i][col] = values[i];
            }
        }

        ///<summary>
	    /// Set one specific value in the matrix.
	    ///</summary>
        public void SetValue(int line, int col, float value) {
            Debug.Assert(line <= I && col <= J, "There is no col " + col + " or line " + line + " for this matrix.");

            matrix[line][col] = value;
        }

        ///<summary>
	    /// Redimension the Matrix by creating a new one an copying all value and returning the new one.
        /// This new Matrix can be set as synapse to get random synapse values in the new empty cell if there are some.
	    ///</summary>
        public static Matrix Redimension(Matrix oldMatrix, int newI, int newJ, bool synapse = true) {

            Matrix redimMat = new Matrix(newI, newJ);

            if(synapse)
                redimMat.SetAsSynapse(); // in order to get random values then we will copy the previous ones
            else
                redimMat.SetToZero();

            int smallI = Mathf.Min(newI, oldMatrix.I);
            int smallJ = Mathf.Min(newJ, oldMatrix.J);

            for (int i = 0; i < smallI; i++) {
                for (int j = 0; j < smallJ; j++) {
                    redimMat.matrix[i][j] = oldMatrix.matrix[i][j];
                }
            }

            return redimMat;
        }
		
	}
}
