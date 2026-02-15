import { useState, useEffect, useCallback } from "react";
import { ShowCard } from "../components/ShowCard";
import { apiClient } from "../api/client";
import { ShowDto, ShowFilterCriteria } from "../types/api";
import { Loader2, Search } from "lucide-react";

export function ShowAllPage() {
  const [shows, setShows] = useState<ShowDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState("");
  const [filters, setFilters] = useState<ShowFilterCriteria>({});

  const loadShows = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      if (searchQuery) {
        const data = await apiClient.searchShows(searchQuery);
        setShows(data);
      } else if (Object.keys(filters).length > 0) {
        const data = await apiClient.filterShows(filters);
        setShows(data);
      } else {
        const data = await apiClient.getAllShows();
        setShows(data);
      }
    } catch (err) {
      setError("Failed to load shows. Please try again later.");
      console.error("Error loading shows:", err);
    } finally {
      setIsLoading(false);
    }
  }, [filters, searchQuery]);

  useEffect(() => {
    loadShows();
  }, [loadShows]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    loadShows();
  };

  const handleFilterChange = (
    key: keyof ShowFilterCriteria,
    value: string | number,
  ) => {
    setFilters((prev) => ({
      ...prev,
      [key]: value || undefined,
    }));
  };

  const applyFilters = () => {
    loadShows();
  };

  const clearFilters = () => {
    setFilters({});
    setSearchQuery("");
    loadShows();
  };

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-3xl font-bold mb-4">All Shows</h1>

        {/* Search */}
        <form onSubmit={handleSearch} className="mb-4">
          <div className="flex gap-2">
            <div className="flex-1 relative">
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Search shows..."
                className="input-field pl-10"
              />
              <Search
                className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"
                size={20}
              />
            </div>
            <button type="submit" className="btn-primary">
              Search
            </button>
          </div>
        </form>

        {/* Filters */}
        <div className="card p-4 mb-6">
          <h3 className="font-semibold mb-3">Filters</h3>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">Type</label>
              <input
                type="text"
                value={filters.type || ""}
                onChange={(e) => handleFilterChange("type", e.target.value)}
                placeholder="e.g., Opera, Ballet"
                className="input-field"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">
                Min Price (PLN)
              </label>
              <input
                type="number"
                value={filters.minPrice || ""}
                onChange={(e) =>
                  handleFilterChange("minPrice", Number(e.target.value))
                }
                placeholder="0"
                className="input-field"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">
                Max Price (PLN)
              </label>
              <input
                type="number"
                value={filters.maxPrice || ""}
                onChange={(e) =>
                  handleFilterChange("maxPrice", Number(e.target.value))
                }
                placeholder="1000"
                className="input-field"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">
                Start Date
              </label>
              <input
                type="date"
                value={filters.startDate || ""}
                onChange={(e) =>
                  handleFilterChange("startDate", e.target.value)
                }
                className="input-field"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">End Date</label>
              <input
                type="date"
                value={filters.endDate || ""}
                onChange={(e) => handleFilterChange("endDate", e.target.value)}
                className="input-field"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">
                Age Restriction
              </label>
              <select
                value={filters.ageRestriction || ""}
                onChange={(e) =>
                  handleFilterChange("ageRestriction", e.target.value)
                }
                className="input-field"
              >
                <option value="">All ages</option>
                <option value="0+">0+</option>
                <option value="6+">6+</option>
                <option value="12+">12+</option>
                <option value="16+">16+</option>
                <option value="18+">18+</option>
              </select>
            </div>
          </div>
          <div className="flex gap-2 mt-4">
            <button onClick={applyFilters} className="btn-primary">
              Apply Filters
            </button>
            <button onClick={clearFilters} className="btn-secondary">
              Clear Filters
            </button>
          </div>
        </div>
      </div>

      {isLoading ? (
        <div className="flex items-center justify-center h-64">
          <Loader2 className="animate-spin text-primary-600" size={48} />
        </div>
      ) : error ? (
        <div className="text-center py-12">
          <p className="text-red-600 dark:text-red-400 mb-4">{error}</p>
          <button onClick={loadShows} className="btn-primary">
            Try Again
          </button>
        </div>
      ) : shows.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-gray-600 dark:text-gray-400">
            No shows found matching your criteria.
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
