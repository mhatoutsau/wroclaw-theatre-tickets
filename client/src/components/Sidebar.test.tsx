import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { BrowserRouter } from "react-router-dom";
import { Sidebar } from "../components/Sidebar";
import { AuthProvider } from "../contexts/AuthContext";
import { ThemeProvider } from "../contexts/ThemeContext";
import * as apiClient from "../api/client";

vi.mock("../api/client", () => ({
  apiClient: {
    getToken: vi.fn(),
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

describe("Sidebar", () => {
  it("displays logo and title", () => {
    vi.mocked(apiClient.apiClient.getToken).mockReturnValue(null);

    renderWithProviders(<Sidebar />);

    expect(screen.getByText("Wrocław Theatre")).toBeInTheDocument();
    expect(screen.getByText("Tickets")).toBeInTheDocument();
  });

  it("shows login and signup buttons when not authenticated", () => {
    vi.mocked(apiClient.apiClient.getToken).mockReturnValue(null);

    renderWithProviders(<Sidebar />);

    expect(screen.getByRole("link", { name: /login/i })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /sign up/i })).toBeInTheDocument();
  });

  it("displays basic navigation links", () => {
    vi.mocked(apiClient.apiClient.getToken).mockReturnValue(null);

    renderWithProviders(<Sidebar />);

    expect(screen.getByRole("link", { name: /home/i })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /show all/i })).toBeInTheDocument();
  });

  it("does not show favorites and profile links when not authenticated", () => {
    vi.mocked(apiClient.apiClient.getToken).mockReturnValue(null);

    renderWithProviders(<Sidebar />);

    expect(
      screen.queryByRole("link", { name: /favorites/i }),
    ).not.toBeInTheDocument();
    expect(
      screen.queryByRole("link", { name: /profile/i }),
    ).not.toBeInTheDocument();
  });

  it("highlights active route", () => {
    vi.mocked(apiClient.apiClient.getToken).mockReturnValue(null);

    renderWithProviders(<Sidebar />);

    // Home link should be active when on home page
    const homeLink = screen.getByRole("link", { name: /home/i });
    expect(homeLink.className).toContain("bg-primary");
  });
});
