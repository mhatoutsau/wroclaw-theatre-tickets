import { useAuth } from '../contexts/AuthContext';
import { Navigate } from 'react-router-dom';
import { User, Mail, Shield } from 'lucide-react';

export function ProfilePage() {
  const { isAuthenticated, user } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }

  return (
    <div>
      <h1 className="text-3xl font-bold mb-6">My Profile</h1>

      <div className="card p-6 max-w-2xl">
        <div className="flex items-center gap-4 mb-6 pb-6 border-b border-gray-200 dark:border-gray-700">
          <div className="w-20 h-20 bg-primary-100 dark:bg-primary-900 rounded-full flex items-center justify-center">
            <User className="text-primary-600 dark:text-primary-400" size={40} />
          </div>
          <div>
            <h2 className="text-2xl font-bold">{user?.email}</h2>
            <div className="flex items-center gap-2 mt-1">
              <Shield size={16} className="text-gray-600 dark:text-gray-400" />
              <span className="text-sm text-gray-600 dark:text-gray-400">
                Role: {user?.role}
              </span>
            </div>
          </div>
        </div>

        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">
              <Mail className="inline mr-2" size={16} />
              Email
            </label>
            <div className="input-field bg-gray-50 dark:bg-gray-900">
              {user?.email}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">User ID</label>
            <div className="input-field bg-gray-50 dark:bg-gray-900 font-mono text-sm">
              {user?.id}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Account Type</label>
            <div className="input-field bg-gray-50 dark:bg-gray-900">
              {user?.role}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
