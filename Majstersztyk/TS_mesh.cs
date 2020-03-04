using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Majstersztyk;

namespace Majstersztyk
{
    public class TS_mesh
    {
        public TS_point CornerTopLeft { get; private set; }
        public TS_point CornerBottomRight { get; private set; }

        private int relativeFactor = 100;
        private TS_section section;

        public double MeshEyeSize { get; private set; }

        public int MeshSize_X { get; private set; }

        public int MeshSize_Y { get; private set; }

        public TS_mesh(TS_section section)
        {
            CornerTopLeft = section.CornerTopLeft;
            CornerBottomRight = section.CornerBottomRight;
            this.section = section;

            MeshEyeSize = Math.Min(CornerTopLeft.Y - CornerBottomRight.Y, CornerBottomRight.X - CornerTopLeft.X) / relativeFactor;
            MeshSize_X = (int)Math.Ceiling((CornerBottomRight.X - CornerTopLeft.X) / MeshEyeSize) + 1;
            MeshSize_Y = (int)Math.Ceiling((CornerTopLeft.Y - CornerBottomRight.Y) / MeshEyeSize) + 1;

            TS_point centralizationVector = new TS_point(
                ((CornerBottomRight.X - CornerTopLeft.X) - (MeshSize_X - 1) * MeshEyeSize) / 2,
                -((CornerTopLeft.Y - CornerBottomRight.Y) - (MeshSize_Y - 1) * MeshEyeSize) / 2
                );

            double[,] matrix = new double[MeshSize_Y, MeshSize_X];

            for (int j = 0; j < MeshSize_X; j++)
            {
                for (int i = 0; i < MeshSize_Y; i++)
                {
                    TS_point point = new TS_point(CornerTopLeft.X + centralizationVector.X + j * MeshEyeSize, 
                        CornerTopLeft.Y + centralizationVector.Y - i * MeshEyeSize);

                    if (section.IsPointInside(point))
                        matrix[i, j] = MeshEyeSize;
                    else
                        matrix[i, j] = 0;

                }
            }

            Nodes = matrix;
        }

        private double[,] Nodes;

        public double TorsionConstant()
        {
            double delta = MeshEyeSize;
            double torsionConstant = 0;
            double currDiff;
            double diff = 1;


            while (diff > 0.000001)
            {
                diff = 0;

                for (int j = 0; j < MeshSize_X; j++)
                {
                    for (int i = 0; i < MeshSize_Y; i++)
                    {
                        if (Nodes[i, j] != 0)
                        {
                            double prevNode = Nodes[i, j];
                            Nodes[i, j] = (Nodes[i - 1, j] + Nodes[i + 1, j] + Nodes[i, j - 1] + Nodes[i, j + 1]
                                + delta * delta) / 4;
                            currDiff = Math.Abs(Nodes[i, j] - prevNode) / Nodes[i, j];
                            diff = Math.Max(currDiff, diff);
                        }
                    }
                }
            }

            torsionConstant = 0;
            foreach (double node in Nodes)
            {
                torsionConstant += node;
            }

            torsionConstant = 4 * delta * delta * torsionConstant;

            return torsionConstant;
        }
    }
}
