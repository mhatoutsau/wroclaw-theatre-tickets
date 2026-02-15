// API Response types based on backend DTOs

export interface TheatreDto {
  id: string;
  name: string;
  address?: string;
  city?: string;
  website?: string;
  phoneNumber?: string;
}

export interface ShowDto {
  id: string;
  title: string;
  description?: string;
  type: string;
  theatre?: TheatreDto;
  director?: string;
  cast?: string;
  startDateTime: string;
  endDateTime?: string;
  duration?: string;
  language?: string;
  minimumPrice?: number;
  maximumPrice?: number;
  ageRestriction: string;
  posterUrl?: string;
  ticketUrl?: string;
  viewCount: number;
  rating: number;
  reviewCount: number;
  isFavorite: boolean;
}

export interface ReviewDto {
  id: string;
  userId: string;
  showId: string;
  rating: number;
  comment?: string;
  createdAt: string;
  isApproved: boolean;
}

export interface ShowDetailDto extends ShowDto {
  fullDescription?: string;
  imageUrl?: string;
  reviews: ReviewDto[];
}

export interface UserDto {
  id: string;
  email: string;
  firstName?: string;
  lastName?: string;
  role: 'User' | 'Moderator' | 'Admin';
}

export interface AuthenticationResponse {
  userId: string;
  email: string;
  accessToken: string;
  refreshToken?: string;
  expiresAt: string;
}

export interface UserRegistrationRequest {
  email: string;
  firstName?: string;
  lastName?: string;
  password: string;
}

export interface UserLoginRequest {
  email: string;
  password: string;
}

export interface ShowFilterCriteria {
  searchQuery?: string;
  type?: string;
  theatreId?: string;
  startDate?: string;
  endDate?: string;
  minPrice?: number;
  maxPrice?: number;
  ageRestriction?: string;
}

export interface CreateReviewRequest {
  showId: string;
  rating: number;
  comment?: string;
}
