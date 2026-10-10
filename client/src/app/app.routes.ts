import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { RegisterComponent } from './components/account/register/register.component';
import { LoginComponent } from './components/account/login/login.component';
import { FooterComponent } from './components/footer/footer.component';
import { NotFoundComponent } from './components/errors/not-found/not-found.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { GamesListComponent } from './components/game/games-list/games-list.component';
import { GamersListComponent } from './components/gamer/gamers-list/gamers-list.component';
import { authGuard } from './guards/auth.guard';
import { authLoggedInGuard } from './guards/auth-logged-in.guard';
import { UserEditComponent } from './components/user/user-edit/user-edit.component';
import { NoAccessComponent } from './components/errors/no-access/no-access.component';
import { ServerErrorComponent } from './components/errors/server-error/server-error.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [
            { path: 'gamer/gamers-list', component: GamersListComponent },
            { path: 'game/games-list', component: GamesListComponent },
            { path: 'user/user-edit', component: UserEditComponent },
            { path: 'no-access', component: NoAccessComponent },
        ]
    },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authLoggedInGuard],
        children: [
            { path: 'account/register', component: RegisterComponent },
            { path: 'account/login', component: LoginComponent },
        ]
    },
    { path: 'footer', component: FooterComponent },
    { path: 'navbar', component: NavbarComponent },
    { path: '**', component: NotFoundComponent, pathMatch: 'full' },
    { path: 'server-error', component: ServerErrorComponent },
];
