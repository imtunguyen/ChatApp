import { NG_EVENT_PLUGINS } from "@taiga-ui/event-plugins";
import { provideAnimations } from "@angular/platform-browser/animations";
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { UNIVERSAL_PROVIDERS } from '@ng-web-apis/universal';
import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideHttpClient } from "@angular/common/http";
import { isPlatformBrowser } from '@angular/common';
export const appConfig: ApplicationConfig = {
  providers:
  [
    provideAnimations(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    NG_EVENT_PLUGINS,
    UNIVERSAL_PROVIDERS,
    provideHttpClient(),

  ]
};
