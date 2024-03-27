package com.clifford.config;

import com.clifford.Service.JwtService;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.NonNull;
import lombok.RequiredArgsConstructor;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;

// This class is a filter that intercepts each request once to perform JWT authentication
@Component
@RequiredArgsConstructor
public class JwtAuthenticationFilter extends OncePerRequestFilter {
    // Injecting the JwtService to handle JWT related operations
    private final JwtService jwtService;
    // Injecting the UserDetailsService to load user-specific data
    private final UserDetailsService userDetailsService;


    // This method is called for every request to perform the authentication
    @Override
    protected void doFilterInternal(@NonNull HttpServletRequest request,
                                    @NonNull HttpServletResponse response, @NonNull FilterChain filterChain) throws ServletException, IOException {
        // Extracting the Authorization header from the request
        final String authorizationHeader = request.getHeader("Authorization");
        final String token;
        final String userEmail;

        // If the Authorization header is not present or does not start with "Bearer ", the filter chain continues with the next filter
        if (authorizationHeader == null || !authorizationHeader.startsWith("Bearer ")) {
            filterChain.doFilter(request, response);
            return;
        }

        // Extracting the JWT token from the Authorization header
        token = authorizationHeader.substring(7);
        // Extracting the user email from the JWT token
        userEmail = jwtService.extractUserEmail(token);

        // If the user email is not null and there is no authentication in the security context
        if (userEmail != null && SecurityContextHolder.getContext().getAuthentication() == null) {
            // Loading the user details
            UserDetails userDetails = this.userDetailsService.loadUserByUsername(userEmail);

            // If the token is valid, an authentication token is created and set in the security context
            if (jwtService.validateToken(token, userDetails)) {
                UsernamePasswordAuthenticationToken authToken = new UsernamePasswordAuthenticationToken(userDetails, null, userDetails.getAuthorities());

                // Setting the authentication details from the request
                authToken.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));

                // Setting the authentication token in the security context
                SecurityContextHolder.getContext().setAuthentication(authToken);
            }
        }
        // The filter chain continues with the next filter
        filterChain.doFilter(request, response);
    }
}