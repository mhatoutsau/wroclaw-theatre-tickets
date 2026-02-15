import { Link, useLocation } from 'react-router-dom';
import { Home, List, Star, User, LogIn, UserPlus, LogOut, Settings } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';

export function Sidebar() {
  const location = useLocation();
  const { isAuthenticated, logout, isAdmin, user } = useAuth();

  const isActive = (path: string) => location.pathname === path;

  const navItems = [
    { path: '/', icon: Home, label: 'Home' },
    { path: '/shows', icon: List, label: 'Show All' },
    ...(isAuthenticated ? [{ path: '/favorites', icon: Star, label: 'Favorites' }] : []),
    ...(isAuthenticated ? [{ path: '/profile', icon: User, label: 'Profile' }] : []),
    ...(isAdmin ? [{ path: '/admin', icon: Settings, label: 'Admin' }] : []),
  ];

  return (
    <aside className="fixed left-0 top-0 h-full w-64 bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700 flex flex-col">
      {/* Logo */}
      <div className="p-6 border-b border-gray-200 dark:border-gray-700">
        <h1 className="text-xl font-bold text-primary-600 dark:text-primary-400">
          Wrocław Theatre
        </h1>
        <p className="text-sm text-gray-600 dark:text-gray-400">Tickets</p>
      </div>

      {/* Navigation */}
      <nav className="flex-1 p-4 space-y-2">
        {navItems.map((item) => {
          const Icon = item.icon;
          return (
            <Link
              key={item.path}
              to={item.path}
              className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                isActive(item.path)
                  ? 'bg-primary-100 dark:bg-primary-900 text-primary-700 dark:text-primary-300'
                  : 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
              }`}
            >
              <Icon size={20} />
              <span className="font-medium">{item.label}</span>
            </Link>
          );
        })}
      </nav>

      {/* Auth buttons */}
      <div className="p-4 border-t border-gray-200 dark:border-gray-700 space-y-2">
        {isAuthenticated ? (
          <>
            <div className="px-4 py-2 text-sm text-gray-600 dark:text-gray-400">
              {user?.email}
            </div>
            <button
              onClick={logout}
              className="flex items-center gap-3 w-full px-4 py-3 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
            >
              <LogOut size={20} />
              <span className="font-medium">Logout</span>
            </button>
          </>
        ) : (
          <>
            <Link
              to="/login"
              className="flex items-center gap-3 w-full px-4 py-3 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
            >
              <LogIn size={20} />
              <span className="font-medium">Login</span>
            </Link>
            <Link
              to="/signup"
              className="flex items-center gap-3 w-full px-4 py-3 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
            >
              <UserPlus size={20} />
              <span className="font-medium">Sign Up</span>
            </Link>
          </>
        )}
      </div>
    </aside>
  );
}
