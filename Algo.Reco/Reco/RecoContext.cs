using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Algo
{
    public class RecoContext
    {
        public IReadOnlyList<User> Users { get; private set; }
        public IReadOnlyList<Movie> Movies { get; private set; }
        public int RatingCount { get; private set; }

        public double Distance( User u1, User u2 )
        {
            bool atLeastOne = false;
            int sum2 = 0;
            foreach( var movieR1 in u1.Ratings )
            {
                if( u2.Ratings.TryGetValue( movieR1.Key, out var r2 ) )
                {
                    atLeastOne = true;
                    sum2 += (movieR1.Value - r2) ^ 2;
                }
            }
            return atLeastOne ? Math.Sqrt( sum2 ) : double.PositiveInfinity;
        }

        public double Similarity( User u1, User u2 ) => 1.0 / (1.0 + Distance( u1, u2 ));

        public double SimilarityPearson( User u1, User u2 )
        {
            // This should call the "real" one below.
            var communMovie = u1.Ratings.Keys.Intersect( u2.Ratings.Keys );
            List<ValueTuple<int,int>> communRatings = new List<ValueTuple<int , int >>();
            foreach(var movie in communMovie )
            {
                u1.Ratings.TryGetValue( movie, out var rate1 );
                u2.Ratings.TryGetValue( movie, out var rate2 );
                communRatings.Add( new ValueTuple<int, int>(rate1, rate2) );
            }

            return SimilarityPearson( communRatings );
        }

        public double SimilarityPearson( IEnumerable<(int x, int y)> values )
        {
            double SumX = 0.0;
            double SumY = 0.0;
            double SumXMultY = 0.0;
            double SumX2 = 0.0;
            double SumY2 = 0.0;

            foreach(var (x, y) in values )
            {
                SumX += x;
                SumY += y;
                SumXMultY += x * y;
                SumX2 += x*x;
                SumY2 += y*y;
            }

            double a = SumXMultY - ((SumX * SumY) / values.Count());
            double b = (SumX2 - ((SumX * SumX) / values.Count())) * (SumY2 - ((SumY * SumY) / values.Count()));
            b = Math.Sqrt( b );

            return a / b;
        }

        public bool LoadFrom( string folder )
        {
            string p = Path.Combine( folder, "users.dat" );
            if( !File.Exists( p ) ) return false;
            Users = User.ReadUsers( p );
            p = Path.Combine( folder, "movies.dat" );
            if( !File.Exists( p ) ) return false;
            Movies = Movie.ReadMovies( p );
            p = Path.Combine( folder, "ratings.dat" );
            if( !File.Exists( p ) ) return false;
            RatingCount = User.ReadRatings( Users, Movies, p );
            return true;
        }

    }
}
