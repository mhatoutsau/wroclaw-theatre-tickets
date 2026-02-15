import { useState, useEffect } from "react";
import { ShowCard } from "../components/ShowCard";
import { apiClient } from "../api/client";
import { ShowDto } from "../types/api";
import { Loader2 } from "lucide-react";

export function HomePage() {
  const [shows, setShows] = useState<ShowDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadShows = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await apiClient.getUpcomingShows(30);
      setShows(data);
    } catch (err) {
      setError("Failed to load shows. Please try again later.");
      console.error("Error loading shows:", err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadShows();
  }, []);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2
          className="animate-spin text-primary-600"
          size={48}
          role="progressbar"
          aria-label="Loading"
        />
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-12">
        <p className="text-red-600 dark:text-red-400 mb-4">{error}</p>
        <button onClick={loadShows} className="btn-primary">
          Try Again
        </button>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-3xl font-bold mb-2">Upcoming Shows</h1>
        <p className="text-gray-600 dark:text-gray-400">
          Discover the best theatre performances in Wrocław for the next 30 days
        </p>
      </div>

      {shows.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-gray-600 dark:text-gray-400">
            No shows available at the moment.
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {shows.map((show) => (
            <ShowCard key={show.id} show={show} onFavoriteChange={loadShows} />
          ))}
        </div>
      )}
    </div>
  );
}
