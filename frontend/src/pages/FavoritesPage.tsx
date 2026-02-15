import { useState, useEffect } from 'react';
import { ShowCard } from '../components/ShowCard';
import { apiClient } from '../api/client';
import { ShowDto } from '../types/api';
import { Loader2, Heart } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { Navigate } from 'react-router-dom';

export function FavoritesPage() {
  const { isAuthenticated } = useAuth();
  const [shows, setShows] = useState<ShowDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadFavorites = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await apiClient.getFavorites();
      setShows(data);
    } catch (err) {
      setError('Failed to load favorites. Please try again later.');
      console.error('Error loading favorites:', err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (isAuthenticated) {
      loadFavorites();
    }
  }, [isAuthenticated]);

  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="animate-spin text-primary-600" size={48} />
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-12">
        <p className="text-red-600 dark:text-red-400 mb-4">{error}</p>
        <button onClick={loadFavorites} className="btn-primary">
          Try Again
        </button>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-6 flex items-center gap-3">
        <Heart className="text-primary-600" size={32} />
        <div>
          <h1 className="text-3xl font-bold">My Favorites</h1>
          <p className="text-gray-600 dark:text-gray-400">
            Your saved shows ({shows.length})
          </p>
        </div>
      </div>

      {shows.length === 0 ? (
        <div className="text-center py-12">
          <Heart className="mx-auto mb-4 text-gray-400" size={64} />
          <p className="text-gray-600 dark:text-gray-400 mb-2">
            You haven't added any favorites yet.
          </p>
          <p className="text-gray-500 dark:text-gray-500 text-sm">
            Click the star icon on any show to add it to your favorites.
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {shows.map((show) => (
            <ShowCard key={show.id} show={show} onFavoriteChange={loadFavorites} />
          ))}
        </div>
      )}
    </div>
  );
}
