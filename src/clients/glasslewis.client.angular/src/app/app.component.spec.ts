import { TestBed, ComponentFixture, fakeAsync, tick } from '@angular/core/testing';
import { Component } from '@angular/core';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { Subject, of } from 'rxjs';
import { CommonModule } from '@angular/common';

import { AppComponent } from './app.component';
import {
  MsalService,
  MsalBroadcastService,
  MSAL_GUARD_CONFIG,
  MsalGuardConfiguration
} from '@azure/msal-angular';
import {
  AuthenticationResult,
  InteractionStatus,
  InteractionType,
  PopupRequest,
  RedirectRequest,
  EventMessage,
  EventType,
  AccountInfo,
  PublicClientApplication
} from '@azure/msal-browser';

// Mock component for router-outlet testing
@Component({
  template: '<div>Mock Route Content</div>'
})
class MockRouteComponent { }

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let mockMsalService: jasmine.SpyObj<MsalService>;
  let mockMsalBroadcastService: jasmine.SpyObj<MsalBroadcastService>;
  let mockMsalInstance: jasmine.SpyObj<PublicClientApplication>;
  let mockMsalGuardConfig: MsalGuardConfiguration;
  
  // Subjects for testing observables
  let inProgressSubject: Subject<InteractionStatus>;
  let msalSubject: Subject<EventMessage>;

  const mockAccount: AccountInfo = {
    homeAccountId: 'test-home-account-id',
    localAccountId: 'test-local-account-id',
    environment: 'test-environment',
    tenantId: 'test-tenant-id',
    username: 'test@example.com',
    name: 'Test User',
    nativeAccountId: 'test-native-account-id',
    authorityType: 'test-authority-type',
    idTokenClaims: {}
  };

  const mockAuthResult: AuthenticationResult = {
    account: mockAccount,
    accessToken: 'test-access-token',
    idToken: 'test-id-token',
    scopes: ['test-scope'],
    uniqueId: 'test-unique-id',
    tenantId: 'test-tenant-id',
    state: 'test-state',
    authority: 'test-authority',
    familyId: 'test-family-id',
    correlationId: 'test-correlation-id',
    expiresOn: new Date(),
    extExpiresOn: new Date(),
    idTokenClaims: {},
    fromCache: false,
    cloudGraphHostName: 'test-cloud-graph',
    msGraphHost: 'test-ms-graph',
    tokenType: 'Bearer'
  };

  beforeEach(async () => {
    // Create subjects for observables
    inProgressSubject = new Subject<InteractionStatus>();
    msalSubject = new Subject<EventMessage>();

    // Create mock MSAL instance
    mockMsalInstance = jasmine.createSpyObj('PublicClientApplication', [
      'initialize',
      'enableAccountStorageEvents',
      'handleRedirectPromise',
      'setActiveAccount',
      'getActiveAccount',
      'getAllAccounts'
    ]);

    // Create mock MSAL services
    mockMsalService = jasmine.createSpyObj('MsalService', [
      'loginPopup',
      'loginRedirect',
      'logoutPopup',
      'logoutRedirect'
    ]);

    mockMsalBroadcastService = jasmine.createSpyObj('MsalBroadcastService', [], {
      inProgress$: inProgressSubject.asObservable(),
      msalSubject$: msalSubject.asObservable()
    });

    // Mock guard configuration
    mockMsalGuardConfig = {
      interactionType: InteractionType.Redirect,
      authRequest: {
        scopes: ['user.read']
      }
    };

    // Setup default mock behaviors
    mockMsalInstance.initialize.and.returnValue(Promise.resolve());
    mockMsalInstance.handleRedirectPromise.and.returnValue(Promise.resolve(null));
    mockMsalInstance.getAllAccounts.and.returnValue([]);
    mockMsalInstance.getActiveAccount.and.returnValue(null);
    mockMsalService.instance = mockMsalInstance;
    mockMsalService.loginPopup.and.returnValue(of(mockAuthResult));
    mockMsalService.loginRedirect.and.returnValue(of(undefined));

    await TestBed.configureTestingModule({
      imports: [
        AppComponent,
        CommonModule,
        MatToolbarModule,
        MatButtonModule,
        RouterTestingModule.withRoutes([
          { path: '', component: MockRouteComponent }
        ])
      ],
      providers: [
        { provide: MsalService, useValue: mockMsalService },
        { provide: MsalBroadcastService, useValue: mockMsalBroadcastService },
        { provide: MSAL_GUARD_CONFIG, useValue: mockMsalGuardConfig }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;

    // Mock window properties
    spyOnProperty(window, 'parent', 'get').and.returnValue(window);
    spyOnProperty(window, 'opener', 'get').and.returnValue(null);
  });

  afterEach(() => {
    inProgressSubject.complete();
    msalSubject.complete();
  });

  describe('Component Creation', () => {
    it('should create the app', () => {
      expect(component).toBeTruthy();
    });

    it('should have the correct title', () => {
      expect(component.title).toEqual('Glass Lewis Coding Challenge');
    });

    it('should initialize with correct default values', () => {
      expect(component.loginDisplay).toBeFalse();
      expect(component.isIframe).toBeFalse();
    });
  });

  describe('ngOnInit', () => {
    it('should initialize MSAL instance', fakeAsync(() => {
      component.ngOnInit();
      tick();

      expect(mockMsalInstance.initialize).toHaveBeenCalled();
      expect(mockMsalInstance.enableAccountStorageEvents).toHaveBeenCalled();
      expect(mockMsalInstance.handleRedirectPromise).toHaveBeenCalled();
    }));

    it('should set active account when redirect promise returns account', fakeAsync(() => {
      mockMsalInstance.handleRedirectPromise.and.returnValue(Promise.resolve(mockAuthResult));

      component.ngOnInit();
      tick();

      expect(mockMsalInstance.setActiveAccount).toHaveBeenCalledWith(mockAccount);
    }));

    it('should not set active account when redirect promise returns null', fakeAsync(() => {
      mockMsalInstance.handleRedirectPromise.and.returnValue(Promise.resolve(null));

      component.ngOnInit();
      tick();

      expect(mockMsalInstance.setActiveAccount).not.toHaveBeenCalled();
    }));

    it('should handle inProgress$ observable for InteractionStatus.None', fakeAsync(() => {
      spyOn(component, 'setLoginDisplay');
      spyOn(component, 'checkAndSetActiveAccount');

      component.ngOnInit();
      tick();

      // Emit InteractionStatus.None
      inProgressSubject.next(InteractionStatus.None);
      tick();

      expect(component.setLoginDisplay).toHaveBeenCalled();
      expect(component.checkAndSetActiveAccount).toHaveBeenCalled();
    }));

    it('should not handle inProgress$ observable for other InteractionStatus', fakeAsync(() => {
      spyOn(component, 'setLoginDisplay');
      spyOn(component, 'checkAndSetActiveAccount');

      component.ngOnInit();
      tick();

      // Emit different status
      inProgressSubject.next(InteractionStatus.Startup);
      tick();

      expect(component.checkAndSetActiveAccount).not.toHaveBeenCalled();
    }));

    it('should handle LOGOUT_SUCCESS event', fakeAsync(() => {
      spyOn(component, 'setLoginDisplay');
      spyOn(component, 'checkAndSetActiveAccount');

      component.ngOnInit();
      tick();

      const logoutEvent: EventMessage = {
        eventType: EventType.LOGOUT_SUCCESS,
        interactionType: InteractionType.Redirect,
        payload: null,
        error: null,
        timestamp: Date.now()
      };

      msalSubject.next(logoutEvent);
      tick();

      expect(component.setLoginDisplay).toHaveBeenCalled();
      expect(component.checkAndSetActiveAccount).toHaveBeenCalled();
    }));

    it('should handle LOGIN_SUCCESS event', fakeAsync(() => {
      component.ngOnInit();
      tick();

      const loginEvent: EventMessage = {
        eventType: EventType.LOGIN_SUCCESS,
        interactionType: InteractionType.Redirect,
        payload: mockAuthResult,
        error: null,
        timestamp: Date.now()
      };

      msalSubject.next(loginEvent);
      tick();

      expect(mockMsalInstance.setActiveAccount).toHaveBeenCalledWith(mockAccount);
    }));

    it('should not handle other event types', fakeAsync(() => {
      component.ngOnInit();
      tick();

      const otherEvent: EventMessage = {
        eventType: EventType.ACQUIRE_TOKEN_START,
        interactionType: InteractionType.Redirect,
        payload: null,
        error: null,
        timestamp: Date.now()
      };

      msalSubject.next(otherEvent);
      tick();

      // Should not trigger any additional calls
      expect(mockMsalInstance.setActiveAccount).not.toHaveBeenCalled();
    }));
  });

  describe('setLoginDisplay', () => {
    it('should set loginDisplay to true when accounts exist', () => {
      mockMsalInstance.getAllAccounts.and.returnValue([mockAccount]);

      component.setLoginDisplay();

      expect(component.loginDisplay).toBeTrue();
    });

    it('should set loginDisplay to false when no accounts exist', () => {
      mockMsalInstance.getAllAccounts.and.returnValue([]);

      component.setLoginDisplay();

      expect(component.loginDisplay).toBeFalse();
    });
  });

  describe('checkAndSetActiveAccount', () => {
    it('should not set active account when active account already exists', () => {
      mockMsalInstance.getActiveAccount.and.returnValue(mockAccount);
      mockMsalInstance.getAllAccounts.and.returnValue([mockAccount]);

      component.checkAndSetActiveAccount();

      expect(mockMsalInstance.setActiveAccount).not.toHaveBeenCalled();
    });

    it('should set first account as active when no active account but accounts exist', () => {
      mockMsalInstance.getActiveAccount.and.returnValue(null);
      mockMsalInstance.getAllAccounts.and.returnValue([mockAccount]);

      component.checkAndSetActiveAccount();

      expect(mockMsalInstance.setActiveAccount).toHaveBeenCalledWith(mockAccount);
    });

    it('should not set active account when no accounts exist', () => {
      mockMsalInstance.getActiveAccount.and.returnValue(null);
      mockMsalInstance.getAllAccounts.and.returnValue([]);

      component.checkAndSetActiveAccount();

      expect(mockMsalInstance.setActiveAccount).not.toHaveBeenCalled();
    });
  });

  describe('login', () => {
    it('should call loginPopup with authRequest when interaction type is Popup and authRequest exists', () => {
      component['msalGuardConfig'] = {
        interactionType: InteractionType.Popup,
        authRequest: { scopes: ['user.read'] }
      };

      component.login();

      expect(mockMsalService.loginPopup).toHaveBeenCalledWith({
        scopes: ['user.read']
      } as PopupRequest);
    });

    it('should call loginPopup without authRequest when interaction type is Popup and no authRequest', () => {
      component['msalGuardConfig'] = {
        interactionType: InteractionType.Popup,
        // authRequest is intentionally omitted
      };

      component.login();

      expect(mockMsalService.loginPopup).toHaveBeenCalledWith();
    });

    it('should set active account after successful popup login', () => {
      component['msalGuardConfig'] = {
        interactionType: InteractionType.Popup,
      };
      mockMsalService.loginPopup.and.returnValue(of(mockAuthResult));

      component.login();

      expect(mockMsalInstance.setActiveAccount).toHaveBeenCalledWith(mockAccount);
    });

    it('should call loginRedirect with authRequest when interaction type is Redirect and authRequest exists', () => {
      component['msalGuardConfig'] = {
        interactionType: InteractionType.Redirect,
        authRequest: { scopes: ['user.read'] }
      };

      component.login();

      expect(mockMsalService.loginRedirect).toHaveBeenCalledWith({
        scopes: ['user.read']
      } as RedirectRequest);
    });

    it('should call loginRedirect with loginRequest when interaction type is Redirect and no authRequest', () => {
      component['msalGuardConfig'] = {
        interactionType: InteractionType.Redirect
      };

      component.login();

      expect(mockMsalService.loginRedirect).toHaveBeenCalled();
    });
  });

  describe('logout', () => {
    beforeEach(() => {
      mockMsalInstance.getActiveAccount.and.returnValue(mockAccount);
    });

    it('should call logoutPopup when interaction type is Popup', () => {
      component['msalGuardConfig'] = {
        interactionType: InteractionType.Popup
      };

      component.logout();

      expect(mockMsalService.logoutPopup).toHaveBeenCalledWith({
        account: mockAccount
      });
    });

    it('should call logoutRedirect when interaction type is Redirect', () => {
      component['msalGuardConfig'] = {
        interactionType: InteractionType.Redirect
      };

      component.logout();

      expect(mockMsalService.logoutRedirect).toHaveBeenCalledWith({
        account: mockAccount
      });
    });
  });

  describe('ngOnDestroy', () => {
    it('should complete the destroying subject', () => {
      spyOn(component['_destroying$'], 'next');
      spyOn(component['_destroying$'], 'complete');

      component.ngOnDestroy();

      expect(component['_destroying$'].next).toHaveBeenCalledWith(undefined);
      expect(component['_destroying$'].complete).toHaveBeenCalled();
    });
  });

  describe('Template Integration', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should render title in toolbar', () => {
      const titleElement = fixture.debugElement.query(By.css('a.title'));
      expect(titleElement.nativeElement.textContent).toContain('Glass Lewis Coding Challenge');
    });

    it('should show login button when not logged in', () => {
      component.loginDisplay = false;
      fixture.detectChanges();

      const loginButton = fixture.debugElement.query(By.css('button[mat-raised-button]:not([color])'));
      const logoutButton = fixture.debugElement.query(By.css('button[color="accent"]'));

      expect(loginButton).toBeTruthy();
      expect(loginButton.nativeElement.textContent).toContain('Login');
      expect(logoutButton).toBeFalsy();
    });

    it('should show logout button when logged in', () => {
      component.loginDisplay = true;
      fixture.detectChanges();

      const loginButton = fixture.debugElement.query(By.css('button[mat-raised-button]:not([color])'));
      const logoutButton = fixture.debugElement.query(By.css('button[color="accent"]'));

      expect(loginButton).toBeFalsy();
      expect(logoutButton).toBeTruthy();
      expect(logoutButton.nativeElement.textContent).toContain('Logout');
    });

    it('should call login when login button is clicked', () => {
      spyOn(component, 'login');
      component.loginDisplay = false;
      fixture.detectChanges();

      const loginButton = fixture.debugElement.query(By.css('button[mat-raised-button]:not([color])'));
      loginButton.nativeElement.click();

      expect(component.login).toHaveBeenCalled();
    });

    it('should call logout when logout button is clicked', () => {
      spyOn(component, 'logout');
      component.loginDisplay = true;
      fixture.detectChanges();

      const logoutButton = fixture.debugElement.query(By.css('button[color="accent"]'));
      logoutButton.nativeElement.click();

      expect(component.logout).toHaveBeenCalled();
    });

    it('should show router-outlet when not in iframe', () => {
      component.isIframe = false;
      fixture.detectChanges();

      const routerOutlet = fixture.debugElement.query(By.css('router-outlet'));
      expect(routerOutlet).toBeTruthy();
    });

    it('should hide router-outlet when in iframe', () => {
      component.isIframe = true;
      fixture.detectChanges();

      const routerOutlet = fixture.debugElement.query(By.css('router-outlet'));
      expect(routerOutlet).toBeFalsy();
    });

    it('should have correct CSS classes and structure', () => {
      const toolbar = fixture.debugElement.query(By.css('mat-toolbar'));
      const spacer = fixture.debugElement.query(By.css('.toolbar-spacer'));
      const container = fixture.debugElement.query(By.css('.container'));

      expect(toolbar).toBeTruthy();
      expect(toolbar.attributes['color']).toBe('primary');
      expect(spacer).toBeTruthy();
      expect(container).toBeTruthy();
    });

    it('should have correct title link attributes', () => {
      const titleLink = fixture.debugElement.query(By.css('a.title'));
      expect(titleLink.attributes['href']).toBe('/');
      expect(titleLink.nativeElement.textContent.trim()).toBe('Glass Lewis Coding Challenge');
    });
  });

  describe('Edge Cases and Error Handling', () => {
    it('should handle null result from handleRedirectPromise', fakeAsync(() => {
      mockMsalInstance.handleRedirectPromise.and.returnValue(Promise.resolve(null));

      component.ngOnInit();
      tick();

      expect(mockMsalInstance.setActiveAccount).not.toHaveBeenCalled();
    }));

    it('should handle result with null account from handleRedirectPromise', fakeAsync(() => {
      const resultWithNullAccount = { ...mockAuthResult, account: null };
      mockMsalInstance.handleRedirectPromise.and.returnValue(Promise.resolve(resultWithNullAccount));

      component.ngOnInit();
      tick();

      expect(mockMsalInstance.setActiveAccount).not.toHaveBeenCalled();
    }));

    it('should handle multiple accounts in checkAndSetActiveAccount', () => {
      const secondAccount: AccountInfo = { ...mockAccount, username: 'second@example.com' };
      mockMsalInstance.getActiveAccount.and.returnValue(null);
      mockMsalInstance.getAllAccounts.and.returnValue([mockAccount, secondAccount]);

      component.checkAndSetActiveAccount();

      expect(mockMsalInstance.setActiveAccount).toHaveBeenCalledWith(mockAccount);
    });

    it('should handle iframe detection with opener window', fakeAsync(() => {
      component.ngOnInit();
      tick();

      expect(component.isIframe).toBeFalse();
    }));
  });

  describe('Integration Tests', () => {
    it('should complete full login flow with popup', fakeAsync(() => {
      component['msalGuardConfig'] = {
        interactionType: InteractionType.Popup,
        // authRequest is intentionally omitted
      };
      mockMsalInstance.getAllAccounts.and.returnValue([]);
      
      component.ngOnInit();
      tick();

      expect(component.loginDisplay).toBeFalse();

      // Simulate login
      component.login();
      tick();

      // Simulate successful login result
      mockMsalInstance.getAllAccounts.and.returnValue([mockAccount]);
      component.setLoginDisplay();

      expect(component.loginDisplay).toBeTrue();
      expect(mockMsalInstance.setActiveAccount).toHaveBeenCalledWith(mockAccount);
    }));

    it('should complete full logout flow', fakeAsync(() => {
      component['msalGuardConfig'] = {
        interactionType: InteractionType.Redirect,
      };
      mockMsalInstance.getAllAccounts.and.returnValue([mockAccount]);
      mockMsalInstance.getActiveAccount.and.returnValue(mockAccount);
      
      component.ngOnInit();
      tick();

      component.setLoginDisplay();
      expect(component.loginDisplay).toBeTrue();

      // Simulate logout
      component.logout();

      // Simulate logout event
      const logoutEvent: EventMessage = {
        eventType: EventType.LOGOUT_SUCCESS,
        interactionType: InteractionType.Redirect,
        payload: null,
        error: null,
        timestamp: Date.now()
      };

      msalSubject.next(logoutEvent);
      tick();

      // Simulate no accounts after logout
      mockMsalInstance.getAllAccounts.and.returnValue([]);
      component.setLoginDisplay();

      expect(component.loginDisplay).toBeFalse();
    }));
  });
});