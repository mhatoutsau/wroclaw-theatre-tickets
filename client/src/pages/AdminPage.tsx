import { useState, useEffect } from "react";
import { apiClient } from "../api/client";
import { ShowDto } from "../types/api";
import { Loader2, Settings, TrendingUp } from "lucide-react";
import { useAuth } from "../contexts/AuthContext";
import { Navigate } from "react-router-dom";

export function AdminPage() {
  const { isAdmin } = useAuth();
  const [shows, setShows] = useState<ShowDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadShows = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await apiClient.getAllShows();
      setShows(data);
    } catch (err) {
      setError("Failed to load shows. Please try again later.");
      console.error("Error loading shows:", err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (isAdmin) {
      loadShows();
    }
  }, [isAdmin]);

  if (!isAdmin) {
    return <Navigate to="/" />;
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
        <button onClick={loadShows} className="btn-primary">
          Try Again
        </button>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-6 flex items-center gap-3">
        <Settings className="text-primary-600" size={32} />
        <div>
          <h1 className="text-3xl font-bold">Admin Dashboard</h1>
          <p className="text-gray-600 dark:text-gray-400">
            Manage shows and review system
          </p>
        </div>
      </div>

      {/* Statistics */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        <div className="card p-6">
          <div className="flex items-center justify-between mb-2">
            <h3 className="text-sm font-medium text-gray-600 dark:text-gray-400">
              Total Shows
            </h3>
            <TrendingUp className="text-primary-600" size={20} />
          </div>
          <p className="text-3xl font-bold">{shows.length}</p>
        </div>

        <div className="card p-6">
          <div className="flex items-center justify-between mb-2">
            <h3 className="text-sm font-medium text-gray-600 dark:text-gray-400">
              Active Shows
            </h3>
            <TrendingUp className="text-green-600" size={20} />
          </div>
          <p className="text-3xl font-bold">
            {shows.filter((s) => new Date(s.startDateTime) > new Date()).length}
          </p>
        </div>

        <div className="card p-6">
          <div className="flex items-center justify-between mb-2">
            <h3 className="text-sm font-medium text-gray-600 dark:text-gray-400">
              Total Views
            </h3>
            <TrendingUp className="text-blue-600" size={20} />
          </div>
          <p className="text-3xl font-bold">
            {shows.reduce((sum, show) => sum + show.viewCount, 0)}
          </p>
        </div>
      </div>

      {/* Shows List */}
      <div className="card p-6">
        <h2 className="text-xl font-bold mb-4">All Shows</h2>
        <div className="space-y-4">
          {shows.map((show) => (
            <div
              key={show.id}
              className="flex items-center justify-between p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors"
            >
              <div className="flex-1">
                <h3 className="font-semibold">{show.title}</h3>
                <p className="text-sm text-gray-600 dark:text-gray-400">
                  {show.theatre?.name} •{" "}
                  {new Date(show.startDateTime).toLocaleDateString()}
                </p>
              </div>
              <div className="flex items-center gap-4">
                <div className="text-right">
                  <p className="text-sm text-gray-600 dark:text-gray-400">
                    Views
                  </p>
                  <p className="font-semibold">{show.viewCount}</p>
                </div>
                <button className="btn-secondary text-sm">Manage</button>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
