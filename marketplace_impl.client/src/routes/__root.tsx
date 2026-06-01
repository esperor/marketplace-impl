import { createRootRoute, Outlet } from '@tanstack/react-router';
import Navigation from '../components/navigation';
import React from 'react';

const TanStackRouterDevtools =
  process.env.NODE_ENV === 'production'
    ? () => null // Render nothing in production
    : React.lazy(() =>
        // Lazy load in development
        import('@tanstack/react-router-devtools').then((res) => ({
          default: res.TanStackRouterDevtools,
          // For Embedded Mode
          // default: res.TanStackRouterDevtoolsPanel
        })),
      );

export const Route = createRootRoute({
  component: () => {
    return (
      <>
        <Navigation />
        <main className="w-screen overflow-auto flex-1 flex flex-col">
          <div
            className="w-[80%] mx-auto flex-1 flex"
          >
            <Outlet />
          </div>
        </main>
        <TanStackRouterDevtools />
      </>
    );
  },
});
