import axios, {
  AxiosInstance,
  AxiosRequestHeaders,
  InternalAxiosRequestConfig,
} from "axios";
import type {
  ShowDto,
  ShowDetailDto,
  AuthenticationResponse,
  UserLoginRequest,
  UserRegistrationRequest,
  ShowFilterCriteria,
  CreateReviewRequest,
  ReviewDto,
} from "../types/api";

class ApiClient {
  private client: AxiosInstance;
  private tokenKey = "auth_token";

  constructor() {
    const baseURL = "/api";

    this.client = axios.create({
      baseURL,
      headers: {
        "Content-Type": "application/json",
      },
      withCredentials: false,
    });

    // Add auth token to requests
    this.client.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const token = this.getToken();
        if (token) {
          // Properly set the Authorization header
          if (!config.headers) {
            config.headers = {} as AxiosRequestHeaders;
          }
          // Set Authorization header with Bearer token
          config.headers["Authorization"] = `Bearer ${token}`;
          console.debug("Authorization header added to request:", config.url);
        }
        return config;
      },
      (error) => Promise.reject(error),
    );

    // Handle 401 errors
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          console.warn(
            "Unauthorized (401) - clearing token and redirecting to login",
          );
          //this.clearToken();
          //window.location.href = "/login";
        }
        return Promise.reject(error);
      },
    );
  }

  // Token management
  setToken(token: string) {
    if (!token) {
      console.error("Attempted to set an empty token");
      return;
    }
    try {
      localStorage.setItem(this.tokenKey, token);
      console.debug("Token stored in localStorage");
    } catch (error) {
      console.error("Failed to store token in localStorage:", error);
    }
  }

  getToken(): string | null {
    try {
      const token = localStorage.getItem(this.tokenKey);
      if (token) {
        console.debug("Retrieved token from localStorage");
      } else {
        console.debug("No token found in localStorage");
      }
      return token;
    } catch (error) {
      console.error("Failed to retrieve token from localStorage:", error);
      return null;
    }
  }

  clearToken() {
    try {
      localStorage.removeItem(this.tokenKey);
      console.debug("Token cleared from localStorage");
    } catch (error) {
      console.error("Failed to clear token from localStorage:", error);
    }
  }

  // Auth endpoints
  async register(
    data: UserRegistrationRequest,
  ): Promise<AuthenticationResponse> {
    const response = await this.client.post<AuthenticationResponse>(
      "/auth/register",
      data,
    );
    return response.data;
  }

  async login(data: UserLoginRequest): Promise<AuthenticationResponse> {
    const response = await this.client.post<AuthenticationResponse>(
      "/auth/login",
      data,
    );
    return response.data;
  }

  // Show endpoints
  async getAllShows(): Promise<ShowDto[]> {
    const response = await this.client.get<ShowDto[]>("/shows");
    return response.data;
  }

  async getShowById(id: string): Promise<ShowDetailDto> {
    const response = await this.client.get<ShowDetailDto>(`/shows/${id}`);
    return response.data;
  }

  async getUpcomingShows(days = 30): Promise<ShowDto[]> {
    const response = await this.client.get<ShowDto[]>(
      `/shows/upcoming?days=${days}`,
    );
    return response.data;
  }

  async searchShows(keyword: string): Promise<ShowDto[]> {
    const response = await this.client.get<ShowDto[]>(
      `/shows/search?keyword=${encodeURIComponent(keyword)}`,
    );
    return response.data;
  }

  async filterShows(criteria: ShowFilterCriteria): Promise<ShowDto[]> {
    const response = await this.client.post<ShowDto[]>(
      "/shows/filter",
      criteria,
    );
    return response.data;
  }

  async getMostViewed(): Promise<ShowDto[]> {
    const response = await this.client.get<ShowDto[]>("/shows/trending/viewed");
    return response.data;
  }

  // Favorites endpoints
  async getFavorites(): Promise<ShowDto[]> {
    const response = await this.client.get<ShowDto[]>("/favorites");
    return response.data;
  }

  async addFavorite(showId: string): Promise<void> {
    await this.client.post(`/favorites/${showId}`);
  }

  async removeFavorite(showId: string): Promise<void> {
    await this.client.delete(`/favorites/${showId}`);
  }

  // Review endpoints
  async createReview(data: CreateReviewRequest): Promise<ReviewDto> {
    const response = await this.client.post<ReviewDto>("/reviews", data);
    return response.data;
  }

  // Admin endpoints
  async approveReview(reviewId: string): Promise<void> {
    await this.client.post(`/admin/reviews/${reviewId}/approve`);
  }
}

export const apiClient = new ApiClient();
