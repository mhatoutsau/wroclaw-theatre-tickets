import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { BrowserRouter } from 'react-router-dom'
import { HomePage } from '../pages/HomePage'
import { AuthProvider } from '../contexts/AuthContext'
import { ThemeProvider } from '../contexts/ThemeContext'
import * as apiClient from '../api/client'
import { mockShows } from '../test/mockData'

// Mock the API client
vi.mock('../api/client', () => ({
  apiClient: {
    getUpcomingShows: vi.fn(),
    getToken: vi.fn(() => null),
  },
}))

const renderWithProviders = (component: React.ReactElement) => {
  return render(
    <BrowserRouter>
      <ThemeProvider>
        <AuthProvider>{component}</AuthProvider>
      </ThemeProvider>
    </BrowserRouter>
  )
}

describe('HomePage', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('displays loading spinner while fetching shows', () => {
    vi.mocked(apiClient.apiClient.getUpcomingShows).mockImplementation(
      () => new Promise(() => {}) // Never resolves
    )

    renderWithProviders(<HomePage />)

    expect(screen.getByRole('progressbar', { hidden: true })).toBeInTheDocument()
  })

  it('displays upcoming shows after loading', async () => {
    vi.mocked(apiClient.apiClient.getUpcomingShows).mockResolvedValue(mockShows)

    renderWithProviders(<HomePage />)

    await waitFor(() => {
      expect(screen.getByText('Swan Lake')).toBeInTheDocument()
      expect(screen.getByText('The Nutcracker')).toBeInTheDocument()
    })
  })

  it('displays correct heading and description', async () => {
    vi.mocked(apiClient.apiClient.getUpcomingShows).mockResolvedValue([])

    renderWithProviders(<HomePage />)

    await waitFor(() => {
      expect(screen.getByText('Upcoming Shows')).toBeInTheDocument()
      expect(
        screen.getByText(/Discover the best theatre performances in Wrocław for the next 30 days/i)
      ).toBeInTheDocument()
    })
  })

  it('calls API with correct parameters', async () => {
    vi.mocked(apiClient.apiClient.getUpcomingShows).mockResolvedValue([])

    renderWithProviders(<HomePage />)

    await waitFor(() => {
      expect(apiClient.apiClient.getUpcomingShows).toHaveBeenCalledWith(30)
    })
  })

  it('displays error message when API call fails', async () => {
    vi.mocked(apiClient.apiClient.getUpcomingShows).mockRejectedValue(
      new Error('Network error')
    )

    renderWithProviders(<HomePage />)

    await waitFor(() => {
      expect(
        screen.getByText(/Failed to load shows. Please try again later./i)
      ).toBeInTheDocument()
    })
  })

  it('displays "no shows" message when no shows are returned', async () => {
    vi.mocked(apiClient.apiClient.getUpcomingShows).mockResolvedValue([])

    renderWithProviders(<HomePage />)

    await waitFor(() => {
      expect(screen.getByText(/No shows available at the moment./i)).toBeInTheDocument()
    })
  })

  it('renders shows in a grid layout', async () => {
    vi.mocked(apiClient.apiClient.getUpcomingShows).mockResolvedValue(mockShows)

    const { container } = renderWithProviders(<HomePage />)

    await waitFor(() => {
      const grid = container.querySelector('.grid')
      expect(grid).toBeInTheDocument()
      expect(grid?.classList.contains('grid-cols-1')).toBe(true)
      expect(grid?.classList.contains('md:grid-cols-2')).toBe(true)
    })
  })

  it('has retry button when error occurs', async () => {
    vi.mocked(apiClient.apiClient.getUpcomingShows).mockRejectedValue(
      new Error('Network error')
    )

    renderWithProviders(<HomePage />)

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /try again/i })).toBeInTheDocument()
    })
  })
})
