import { createRootRoute, Outlet } from '@tanstack/react-router';
import Navigation from '../components/navigation';
import React from 'react';
import { node } from '../utils/node';

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

const shrinkNav = () => {
  if (document.readyState !== 'complete') return;
  const nav = node('nav') as HTMLElement;
  const main = node('main') as HTMLElement;
  const innerNav = node('#inner-nav') as HTMLElement;
  if (!main || !nav || !innerNav) return;
  
  innerNav.style.paddingRight = String(nav.clientWidth - main.clientWidth) + 'px';
}

const onLoadMain = () => {
  shrinkNav();
}

export const Route = createRootRoute({
  component: () => {
    return (
      <>
        <Navigation />
        <main className="w-screen overflow-auto flex-1 flex flex-col" onLoad={onLoadMain}>
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
