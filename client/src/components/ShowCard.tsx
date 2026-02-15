import { Star } from 'lucide-react';
import { format } from 'date-fns';
import { ShowDto } from '../types/api';
import { useAuth } from '../contexts/AuthContext';
import { apiClient } from '../api/client';
import { useState } from 'react';

interface ShowCardProps {
  show: ShowDto;
  onFavoriteChange?: () => void;
}

export function ShowCard({ show, onFavoriteChange }: ShowCardProps) {
  const { isAuthenticated } = useAuth();
  const [isFavorite, setIsFavorite] = useState(show.isFavorite);
  const [isLoading, setIsLoading] = useState(false);

  const handleFavoriteToggle = async (e: React.MouseEvent) => {
    e.preventDefault();
    if (!isAuthenticated || isLoading) return;

    setIsLoading(true);
    try {
      if (isFavorite) {
        await apiClient.removeFavorite(show.id);
      } else {
        await apiClient.addFavorite(show.id);
      }
      setIsFavorite(!isFavorite);
      onFavoriteChange?.();
    } catch (error) {
      console.error('Error toggling favorite:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const formatPrice = () => {
    if (!show.minimumPrice && !show.maximumPrice) return 'Price not available';
    if (show.minimumPrice === show.maximumPrice) return `${show.minimumPrice} PLN`;
    return `${show.minimumPrice || 0} - ${show.maximumPrice || 0} PLN`;
  };

  return (
    <div className="card relative">
      {/* Favorite button */}
      {isAuthenticated && (
        <button
          onClick={handleFavoriteToggle}
          disabled={isLoading}
          className="absolute top-3 right-3 z-10 p-2 rounded-full bg-white dark:bg-gray-800 shadow-md hover:scale-110 transition-transform disabled:opacity-50"
        >
          <Star
            size={20}
            className={isFavorite ? 'fill-yellow-400 text-yellow-400' : 'text-gray-400'}
          />
        </button>
      )}

      {/* Poster */}
      {show.posterUrl && (
        <div className="aspect-[3/4] overflow-hidden bg-gray-200 dark:bg-gray-700">
          <img
            src={show.posterUrl}
            alt={show.title}
            className="w-full h-full object-cover"
            onError={(e) => {
              e.currentTarget.style.display = 'none';
            }}
          />
        </div>
      )}

      {/* Content */}
      <div className="p-4">
        <h3 className="font-bold text-lg mb-2 line-clamp-2">{show.title}</h3>
        
        {show.theatre && (
          <p className="text-sm text-gray-600 dark:text-gray-400 mb-1">
            {show.theatre.name}
          </p>
        )}

        {show.theatre?.address && (
          <p className="text-sm text-gray-600 dark:text-gray-400 mb-2">
            {show.theatre.address}
          </p>
        )}

        <p className="text-sm text-gray-600 dark:text-gray-400 mb-2">
          {format(new Date(show.startDateTime), 'PPp')}
        </p>

        <p className="font-semibold text-primary-600 dark:text-primary-400 mb-3">
          {formatPrice()}
        </p>

        {show.description && (
          <p className="text-sm text-gray-700 dark:text-gray-300 mb-3 line-clamp-3">
            {show.description}
          </p>
        )}

        {show.ticketUrl && (
          <a
            href={show.ticketUrl}
            target="_blank"
            rel="noopener noreferrer"
            className="btn-primary w-full text-center block"
          >
            Buy Ticket
          </a>
        )}
      </div>
    </div>
  );
}
