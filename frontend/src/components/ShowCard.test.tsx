import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { BrowserRouter } from "react-router-dom";
import { ShowCard } from "../components/ShowCard";
import { AuthProvider } from "../contexts/AuthContext";
import { ThemeProvider } from "../contexts/ThemeContext";
import { mockShow } from "../test/mockData";
import * as apiClient from "../api/client";

const validToken =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMTIzIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoidGVzdEBleGFtcGxlLmNvbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIifQ.test";

// Mock the API client
vi.mock("../api/client", () => ({
  apiClient: {
    addFavorite: vi.fn(),
    removeFavorite: vi.fn(),
    getToken: vi.fn(() => validToken),
    clearToken: vi.fn(),
    setToken: vi.fn(),
  },
}));

const renderWithProviders = (component: React.ReactElement) => {
  return render(
    <BrowserRouter>
      <ThemeProvider>
        <AuthProvider>{component}</AuthProvider>
      </ThemeProvider>
    </BrowserRouter>,
  );
};

describe("ShowCard", () => {
  it("renders show information correctly", () => {
    renderWithProviders(<ShowCard show={mockShow} />);

    expect(screen.getByText("Swan Lake")).toBeInTheDocument();
    expect(screen.getByText("Opera Wrocławska")).toBeInTheDocument();
    expect(screen.getByText(/50 - 200 PLN/)).toBeInTheDocument();
  });

  it("displays buy button when ticketUrl is provided", () => {
    renderWithProviders(<ShowCard show={mockShow} />);

    const buyButton = screen.getByRole("link", { name: /buy ticket/i });
    expect(buyButton).toBeInTheDocument();
    expect(buyButton).toHaveAttribute("href", mockShow.ticketUrl);
  });

  it("does not display buy button when ticketUrl is missing", () => {
    const showWithoutUrl = { ...mockShow, ticketUrl: undefined };
    renderWithProviders(<ShowCard show={showWithoutUrl} />);

    expect(
      screen.queryByRole("link", { name: /buy ticket/i }),
    ).not.toBeInTheDocument();
  });

  it("displays theater address when available", () => {
    renderWithProviders(<ShowCard show={mockShow} />);

    expect(screen.getByText("Świdnicka 35")).toBeInTheDocument();
  });

  it("formats price correctly when both min and max are equal", () => {
    const showWithSamePrice = {
      ...mockShow,
      minimumPrice: 100,
      maximumPrice: 100,
    };
    renderWithProviders(<ShowCard show={showWithSamePrice} />);

    expect(screen.getByText("100 PLN")).toBeInTheDocument();
  });

  it('shows "Price not available" when no price is set', () => {
    const showWithoutPrice = {
      ...mockShow,
      minimumPrice: undefined,
      maximumPrice: undefined,
    };
    renderWithProviders(<ShowCard show={showWithoutPrice} />);

    expect(screen.getByText("Price not available")).toBeInTheDocument();
  });

  it("displays poster image when posterUrl is provided", () => {
    renderWithProviders(<ShowCard show={mockShow} />);

    const image = screen.getByAltText("Swan Lake");
    expect(image).toBeInTheDocument();
    expect(image).toHaveAttribute("src", mockShow.posterUrl);
  });

  it("truncates long descriptions", () => {
    const { container } = renderWithProviders(<ShowCard show={mockShow} />);

    const description = container.querySelector(".line-clamp-3");
    expect(description).toBeInTheDocument();
  });

  it("formats date and time correctly", () => {
    renderWithProviders(<ShowCard show={mockShow} />);

    // Date should be formatted using date-fns
    expect(screen.getByText(/Mar 15, 2026/i)).toBeInTheDocument();
  });
});

describe("ShowCard - Favorite functionality", () => {
  it("shows favorite button only when user is authenticated", async () => {
    // Mock authenticated state
    vi.spyOn(Storage.prototype, "getItem").mockReturnValue("mock-token");

    renderWithProviders(<ShowCard show={mockShow} />);

    await waitFor(() => {
      const favoriteButtons = screen.queryAllByRole("button");
      expect(favoriteButtons.length).toBeGreaterThan(0);
    });
  });

  it("calls addFavorite when star is clicked and show is not favorited", async () => {
    vi.spyOn(Storage.prototype, "getItem").mockReturnValue("mock-token");
    const mockAddFavorite = vi.fn().mockResolvedValue(undefined);
    vi.mocked(apiClient.apiClient.addFavorite).mockImplementation(
      mockAddFavorite,
    );

    renderWithProviders(<ShowCard show={mockShow} />);

    await waitFor(() => {
      const favoriteButton = screen.getByRole("button");
      expect(favoriteButton).toBeInTheDocument();
    });

    const favoriteButton = screen.getByRole("button");
    fireEvent.click(favoriteButton);

    await waitFor(() => {
      expect(mockAddFavorite).toHaveBeenCalledWith(mockShow.id);
    });
  });

  it("calls removeFavorite when star is clicked and show is favorited", async () => {
    vi.spyOn(Storage.prototype, "getItem").mockReturnValue("mock-token");
    const mockRemoveFavorite = vi.fn().mockResolvedValue(undefined);
    vi.mocked(apiClient.apiClient.removeFavorite).mockImplementation(
      mockRemoveFavorite,
    );

    const favoritedShow = { ...mockShow, isFavorite: true };
    renderWithProviders(<ShowCard show={favoritedShow} />);

    await waitFor(() => {
      const favoriteButton = screen.getByRole("button");
      expect(favoriteButton).toBeInTheDocument();
    });

    const favoriteButton = screen.getByRole("button");
    fireEvent.click(favoriteButton);

    await waitFor(() => {
      expect(mockRemoveFavorite).toHaveBeenCalledWith(mockShow.id);
    });
  });
});
