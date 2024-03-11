package com.clifford.Service;

import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jwts;

import java.security.Key;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import java.util.function.Function;

import io.jsonwebtoken.SignatureAlgorithm;
import io.jsonwebtoken.io.Decoders;
import io.jsonwebtoken.security.Keys;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.stereotype.Service;

@Service
public class JwtService {

    private static final String SECRET_KEY = " 404E635266556A586E3272357538782F413F4428472B4B6250645367566B5970";


    // extract user email from token
    // This method takes a token and returns the email address of the user
    public String extractUserEmail(String token) {
        return extractClaim(token, Claims::getSubject);
    }

    // extract single claim from token
    // This method takes a token and a function that extracts a claim from the token and returns the claim as a string

    public <T> T extractClaim(String token, Function<Claims, T> claimsResolver) {
        final Claims claims = extractAllClaims(token);
        return claimsResolver.apply(claims);
    }


    // generate token for user without extra claims
    // This method takes a UserDetails object and returns a token
    public String generateToken(UserDetails userDetails) {
        return generateToken(new HashMap<>(), userDetails);
    }


    // generate token for user with extra claims
    // This method takes a map of extra claims and a UserDetails object and returns a token
    public String generateToken(
            Map<String, Object> extraClaims,
            UserDetails userDetails
    ) {
        return Jwts
                .builder()
                .setClaims(extraClaims)
                .setSubject(userDetails.getUsername())
                .setIssuedAt(new Date(System.currentTimeMillis()))
                .setExpiration(new Date(System.currentTimeMillis() + 1000 * 60 * 60 * 10))
                .signWith(getSignInKey(), SignatureAlgorithm.HS256)
                .compact();
    }

    // validate token
    // This method takes a token and a UserDetails object and returns a boolean indicating whether the token is valid
    public Boolean validateToken(String token, UserDetails userDetails) {
        final String userEmail = extractUserEmail(token);
        return (userEmail.equals(userDetails.getUsername()) && !isTokenExpired(token));
    }

    // check if the token is expired
    // This method takes a token and returns a boolean indicating whether the token is expired
    private boolean isTokenExpired(String token) {
        return extractExpiration(token).before(new Date());

    }

    // extract expiration date from token
    // This method takes a token and returns the expiration date of the token
    private Date extractExpiration(String token) {
        return extractClaim(token, Claims::getExpiration);
    }


    // this method returns a Key object that is used to sign the token
    // This method returns a Key object that is used to sign the token
    private Claims extractAllClaims(String token) {
        return Jwts
                .parserBuilder()
                .setSigningKey(getSignInKey())
                .build()
                .parseClaimsJws(token)
                .getBody();
    }


    // Key object that is used to sign the token
    // This method returns a Key object that is used to sign the token
    private Key getSignInKey() {
        byte[] keyBytes = Decoders.BASE64.decode(SECRET_KEY);
        return Keys.hmacShaKeyFor(keyBytes);
    }
}
