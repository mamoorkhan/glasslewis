import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';

import {
  MSAL_INSTANCE,
  MSAL_GUARD_CONFIG,
  MSAL_INTERCEPTOR_CONFIG,
  MsalService,
  MsalGuard,
  MsalBroadcastService,
  MsalInterceptor,
  ProtectedResourceScopes,
  MsalInterceptorConfiguration
} from '@azure/msal-angular';
import {
  PublicClientApplication,
  InteractionType,
} from '@azure/msal-browser';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { msalConfig, protectedResources, loginRequest } from './app/auth-config';

function MSALInstanceFactory() {
  return new PublicClientApplication(msalConfig);
}

function MSALGuardConfigFactory() {
  return {
    interactionType: InteractionType.Redirect,
    authRequest: {
      scopes: loginRequest.scopes,
    },
  };
}

function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
    const protectedResourceMap = new Map<string, (string | ProtectedResourceScopes)[] | null>();

    protectedResourceMap.set(protectedResources.companyAPI.endpoint, [
      {
        httpMethod: 'GET',
        scopes: [...protectedResources.companyAPI.scopes.read]
      },
      {
        httpMethod: 'POST',
        scopes: [...protectedResources.companyAPI.scopes.write]
      },
      {
        httpMethod: 'PUT',
        scopes: [...protectedResources.companyAPI.scopes.write]
      },
      {
        httpMethod: 'PATCH',
        scopes: [...protectedResources.companyAPI.scopes.write]
      },
      {
        httpMethod: 'DELETE',
        scopes: [...protectedResources.companyAPI.scopes.write]
      }
    ]);

    return {
      interactionType: InteractionType.Popup,
      protectedResourceMap,
    };
}

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    provideRouter(routes),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true,
    },
    { provide: MSAL_INSTANCE, useFactory: MSALInstanceFactory },
    { provide: MSAL_GUARD_CONFIG, useFactory: MSALGuardConfigFactory },
    { provide: MSAL_INTERCEPTOR_CONFIG, useFactory: MSALInterceptorConfigFactory },
    MsalService,
    MsalGuard,
    MsalBroadcastService,
  ],
}).catch(err => console.error(err));