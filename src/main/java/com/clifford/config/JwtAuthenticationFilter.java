package com.clifford.config;

import com.clifford.Service.JwtService;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.NonNull;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;

@Component
@RequiredArgsConstructor
public class JwtAuthenticationFilter extends OncePerRequestFilter {

     private final JwtService jwtService;
    @Override
    protected void doFilterInternal(
            @NonNull HttpServletRequest request,
            @NonNull HttpServletResponse response,
            @NonNull FilterChain filterChain)
            throws ServletException, IOException {
        final String authorizationHeader = request.getHeader("Authorization");
        final String token;
        final String userEmail;


        // If the token is null or does not start with "Bearer ", then the filter chain is continued
        if (authorizationHeader == null || !authorizationHeader.startsWith("Bearer ")) {

            // The filter chain is continued and the request and response are passed to the next filter
            filterChain.doFilter(request, response);
            return;

        }

        // The token is set to the substring of the authorization header starting at the 7th character
        // This is the token without the "Bearer " prefix
        token = authorizationHeader.substring(7);

        // The user email is set to the email address extracted from the token

        userEmail = jwtService.extractUserEmail(token);


    }

}
