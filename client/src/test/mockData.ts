import { ShowDto } from '../types/api'

export const mockShow: ShowDto = {
  id: '123e4567-e89b-12d3-a456-426614174000',
  title: 'Swan Lake',
  description: 'A classical ballet performance',
  type: 'Ballet',
  theatre: {
    id: '123e4567-e89b-12d3-a456-426614174001',
    name: 'Opera Wrocławska',
    address: 'Świdnicka 35',
    city: 'Wrocław',
    website: 'https://opera.wroclaw.pl',
    phoneNumber: '+48 71 370 88 80',
  },
  director: 'John Smith',
  cast: 'Jane Doe, Bob Wilson',
  startDateTime: '2026-03-15T19:00:00Z',
  endDateTime: '2026-03-15T21:30:00Z',
  duration: '02:30:00',
  language: 'POL',
  minimumPrice: 50,
  maximumPrice: 200,
  ageRestriction: '12+',
  posterUrl: 'https://example.com/poster.jpg',
  ticketUrl: 'https://example.com/tickets',
  viewCount: 150,
  rating: 4.5,
  reviewCount: 12,
  isFavorite: false,
}

export const mockShows: ShowDto[] = [
  mockShow,
  {
    ...mockShow,
    id: '223e4567-e89b-12d3-a456-426614174000',
    title: 'The Nutcracker',
    startDateTime: '2026-03-20T19:00:00Z',
    isFavorite: true,
  },
]

export const mockAuthResponse = {
  userId: '323e4567-e89b-12d3-a456-426614174000',
  email: 'test@example.com',
  accessToken: 'mock-jwt-token',
  refreshToken: 'mock-refresh-token',
  expiresAt: '2026-02-15T10:00:00Z',
}
